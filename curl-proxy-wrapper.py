#!/usr/bin/env python3
"""
Simple HTTP/HTTPS proxy wrapper that uses curl for all requests.
This bypasses .NET's HttpClient issues with JWT proxy authentication.
"""
import sys
import subprocess
import re
from http.server import HTTPServer, BaseHTTPRequestHandler
from urllib.parse import urlparse

UPSTREAM_PROXY = "http://container_container_011CUTqkhLEPvBpWrHc3vovJ--cool-lone-secret-steps:jwt_eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NiIsImtpZCI6Iks3dlRfYUVsdXIySGdsYVJ0QWJ0UThDWDU4dFFqODZIRjJlX1VsSzZkNEEifQ.eyJpc3MiOiJhbnRocm9waWMtZWdyZXNzLWNvbnRyb2wiLCJvcmdhbml6YXRpb25fdXVpZCI6IjVhZDRjNGM1LTg4ZWEtNGI2MS05Nzc2LTAzNGY3ZDZlYjVkNyIsImlhdCI6MTc2MTQwNDA0MiwNXhwIjoxNzYxNDE4NDQyLCJhbGxvd2VkX2hvc3RzIjoiKiIsInNlc3Npb25faWQiOiJzZXNzaW9uXzAxMUNVVHFrZm15Z2NRcldRU1hwWG5uayJ9.jwz8GRSgAWQtpS5kYGqMovPZnaSF6I5IeK5twGqAF81373vxclnLM32rH33z3G3dGNdKJ4_q4X9vT2pBnI-URvA@21.0.0.163:15004"

class ProxyHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        self._proxy_request()

    def do_POST(self):
        self._proxy_request()

    def do_HEAD(self):
        self._proxy_request()

    def do_CONNECT(self):
        # For HTTPS tunneling, just forward the raw connection
        # In practice, we'll use curl for the actual HTTPS request
        self.send_response(200, 'Connection Established')
        self.end_headers()
        # Note: This is simplified - real CONNECT would tunnel the connection

    def _proxy_request(self):
        try:
            url = self.path
            if not url.startswith('http'):
                url = f"http://{self.headers['Host']}{self.path}"

            # Build curl command
            cmd = ['curl', '-s', '-i', '-X', self.command]
            cmd.extend(['-x', UPSTREAM_PROXY])

            # Add headers
            for header, value in self.headers.items():
                if header.lower() not in ['host', 'connection', 'proxy-connection']:
                    cmd.extend(['-H', f'{header}: {value}'])

            # Add body for POST
            if self.command == 'POST':
                content_length = int(self.headers.get('Content-Length', 0))
                if content_length > 0:
                    body = self.rfile.read(content_length)
                    cmd.extend(['--data-binary', '@-'])
                    result = subprocess.run(cmd + [url], input=body, capture_output=True)
                else:
                    result = subprocess.run(cmd + [url], capture_output=True)
            else:
                result = subprocess.run(cmd + [url], capture_output=True)

            # Parse response
            response = result.stdout.decode('utf-8', errors='replace')
            parts = response.split('\r\n\r\n', 1)

            if len(parts) == 2:
                headers_part, body_part = parts
            else:
                headers_part = parts[0]
                body_part = ''

            # Parse status line
            status_line = headers_part.split('\r\n')[0]
            status_match = re.search(r'HTTP/[\d.]+ (\d+)', status_line)
            status_code = int(status_match.group(1)) if status_match else 200

            # Send response
            self.send_response(status_code)

            # Send headers
            for line in headers_part.split('\r\n')[1:]:
                if ':' in line:
                    key, value = line.split(':', 1)
                    key = key.strip()
                    value = value.strip()
                    if key.lower() not in ['connection', 'transfer-encoding']:
                        self.send_header(key, value)

            self.end_headers()
            self.wfile.write(body_part.encode('utf-8'))

        except Exception as e:
            print(f"Error: {e}", file=sys.stderr)
            self.send_error(500, str(e))

def run(port=8888):
    server = HTTPServer(('127.0.0.1', port), ProxyHandler)
    print(f"Proxy server running on http://127.0.0.1:{port}")
    print(f"Upstream proxy: {UPSTREAM_PROXY[:80]}...")
    server.serve_forever()

if __name__ == '__main__':
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8888
    run(port)
