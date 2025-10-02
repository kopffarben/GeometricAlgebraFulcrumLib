// GA-FuL Documentation - Navigation and Language Switcher

(function() {
    'use strict';

    // Mobile Navigation Toggle
    const navToggle = document.getElementById('navToggle');
    const navMenu = document.getElementById('navMenu');

    if (navToggle && navMenu) {
        navToggle.addEventListener('click', function() {
            navMenu.classList.toggle('active');

            // Animate hamburger
            const hamburger = this.querySelector('.hamburger');
            if (hamburger) {
                hamburger.style.transform = navMenu.classList.contains('active')
                    ? 'rotate(45deg)'
                    : 'rotate(0deg)';
            }
        });

        // Close mobile menu when clicking outside
        document.addEventListener('click', function(event) {
            if (!navToggle.contains(event.target) && !navMenu.contains(event.target)) {
                navMenu.classList.remove('active');
                const hamburger = navToggle.querySelector('.hamburger');
                if (hamburger) {
                    hamburger.style.transform = 'rotate(0deg)';
                }
            }
        });

        // Close mobile menu when clicking a link
        const navLinks = navMenu.querySelectorAll('.nav-link');
        navLinks.forEach(function(link) {
            link.addEventListener('click', function() {
                navMenu.classList.remove('active');
                const hamburger = navToggle.querySelector('.hamburger');
                if (hamburger) {
                    hamburger.style.transform = 'rotate(0deg)';
                }
            });
        });
    }

    // Smooth Scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(function(anchor) {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });

                // Update URL without jumping
                if (history.pushState) {
                    history.pushState(null, null, this.getAttribute('href'));
                }
            }
        });
    });

    // Table of Contents auto-highlighting
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
            const id = entry.target.getAttribute('id');
            const tocLink = document.querySelector(`.content a[href="#${id}"]`);

            if (tocLink) {
                if (entry.isIntersecting) {
                    tocLink.classList.add('active-section');
                } else {
                    tocLink.classList.remove('active-section');
                }
            }
        });
    }, {
        rootMargin: '-20% 0px -80% 0px'
    });

    // Observe all headings
    document.querySelectorAll('.content h2, .content h3').forEach(function(heading) {
        if (heading.id) {
            observer.observe(heading);
        }
    });

    // Add copy button to code blocks
    const codeBlocks = document.querySelectorAll('.content pre');
    codeBlocks.forEach(function(block) {
        const button = document.createElement('button');
        button.className = 'copy-button';
        button.textContent = 'Copy';
        button.setAttribute('aria-label', 'Copy code to clipboard');

        button.addEventListener('click', function() {
            const code = block.querySelector('code');
            if (code) {
                const text = code.textContent;

                navigator.clipboard.writeText(text).then(function() {
                    button.textContent = 'Copied!';
                    button.style.background = '#10b981';

                    setTimeout(function() {
                        button.textContent = 'Copy';
                        button.style.background = '';
                    }, 2000);
                }).catch(function(err) {
                    console.error('Failed to copy:', err);
                    button.textContent = 'Error';
                    setTimeout(function() {
                        button.textContent = 'Copy';
                    }, 2000);
                });
            }
        });

        // Position button
        block.style.position = 'relative';
        block.appendChild(button);
    });

    // Language Preference Storage
    function saveLanguagePreference(lang) {
        try {
            localStorage.setItem('gaful-docs-lang', lang);
        } catch (e) {
            // LocalStorage not available
        }
    }

    function getLanguagePreference() {
        try {
            return localStorage.getItem('gaful-docs-lang');
        } catch (e) {
            return null;
        }
    }

    // Detect current language from URL
    function getCurrentLanguage() {
        const path = window.location.pathname;
        if (path.includes('.de.html') || path.includes('.de/')) {
            return 'de';
        }
        return 'en';
    }

    // Save language preference when switching
    const langBtn = document.querySelector('.lang-btn');
    if (langBtn) {
        langBtn.addEventListener('click', function() {
            const currentLang = getCurrentLanguage();
            const newLang = currentLang === 'en' ? 'de' : 'en';
            saveLanguagePreference(newLang);
        });
    }

    // Auto-redirect based on preference (only on homepage)
    if (window.location.pathname.endsWith('index.html') ||
        window.location.pathname.endsWith('/')) {
        const preferredLang = getLanguagePreference();
        const browserLang = navigator.language.substring(0, 2);

        if (preferredLang) {
            // Redirect to preferred language
            const targetPage = preferredLang === 'de' ? 'README.de.html' : 'README.en.html';
            // Uncomment to enable auto-redirect:
            // window.location.href = targetPage;
        } else if (browserLang === 'de') {
            // Auto-suggest German for German browsers
            // window.location.href = 'README.de.html';
        }
    }

    // Add CSS for copy button dynamically
    const style = document.createElement('style');
    style.textContent = `
        .copy-button {
            position: absolute;
            top: 8px;
            right: 8px;
            padding: 4px 12px;
            background: var(--primary-color);
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.85rem;
            transition: all 0.2s ease;
            z-index: 10;
        }

        .copy-button:hover {
            background: var(--primary-dark);
            transform: translateY(-1px);
        }

        .active-section {
            font-weight: 600;
            color: var(--primary-color) !important;
        }
    `;
    document.head.appendChild(style);

    // Add print styles
    const printStyle = document.createElement('style');
    printStyle.media = 'print';
    printStyle.textContent = `
        @media print {
            .navbar, .footer, .lang-switcher, .page-nav, .copy-button {
                display: none !important;
            }

            .content {
                box-shadow: none;
                padding: 0;
            }

            .content a {
                text-decoration: underline;
                color: #000;
            }

            .content a[href^="http"]:after {
                content: " (" attr(href) ")";
                font-size: 0.8em;
                font-style: italic;
            }
        }
    `;
    document.head.appendChild(printStyle);

    console.log('GA-FuL Documentation initialized');
})();
