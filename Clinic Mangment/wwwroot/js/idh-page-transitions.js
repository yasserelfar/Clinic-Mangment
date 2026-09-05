(function () {
    var progressBar = document.getElementById('idh-progress-bar');
    var supportsViewTransitions = typeof document.startViewTransition === 'function';

    function showProgress() {
        if (!progressBar) return;
        progressBar.style.width = '0%';
        progressBar.classList.add('active');
        requestAnimationFrame(function () {
            progressBar.style.width = '70%';
        });
    }

    function hideProgress() {
        if (!progressBar) return;
        progressBar.style.width = '100%';
        setTimeout(function () {
            progressBar.classList.remove('active');
            progressBar.style.width = '0%';
        }, 250);
    }

    function isSameOrigin(url) {
        try {
            var u = new URL(url, window.location.href);
            return u.origin === window.location.origin;
        } catch (e) {
            return false;
        }
    }

    function swapContent(html, url) {
        var parser = new DOMParser();
        var doc = parser.parseFromString(html, 'text/html');

        var newView = doc.getElementById('idh-view');
        var currentView = document.getElementById('idh-view');
        if (!newView || !currentView) {
            window.location.href = url;
            return;
        }

        currentView.innerHTML = newView.innerHTML;
        document.title = doc.title;

        // re-execute any <script> tags that came with the new content
        // (innerHTML does not run them automatically)
        currentView.querySelectorAll('script').forEach(function (oldScript) {
            var newScript = document.createElement('script');
            Array.from(oldScript.attributes).forEach(function (attr) {
                newScript.setAttribute(attr.name, attr.value);
            });
            newScript.textContent = oldScript.textContent;
            oldScript.replaceWith(newScript);
        });

        window.scrollTo(0, 0);

        if (window.IDHDashboard && typeof window.IDHDashboard.init === 'function') {
            window.IDHDashboard.init();
        }

        if (window.IDHTimeline && typeof window.IDHTimeline.init === 'function') {
            window.IDHTimeline.init();
        }
    }

    function navigate(url, addHistory) {
        showProgress();

        fetch(url, { headers: { 'X-Requested-With': 'idh-page-transitions' } })
            .then(function (res) {
                if (!res.ok) throw new Error('Navigation failed');
                return res.text();
            })
            .then(function (html) {
                if (addHistory) {
                    window.history.pushState({ idhTransition: true }, '', url);
                }

                if (supportsViewTransitions) {
                    document.startViewTransition(function () {
                        swapContent(html, url);
                    });
                } else {
                    swapContent(html, url);
                }
            })
            .catch(function () {
                window.location.href = url;
            })
            .finally(hideProgress);
    }

    document.addEventListener('click', function (e) {
        var link = e.target.closest('a');
        if (!link) return;

        var href = link.getAttribute('href');
        if (!href || href.startsWith('#') || href.startsWith('mailto:') || href.startsWith('tel:')) return;
        if (link.target === '_blank' || link.hasAttribute('download')) return;
        if (link.hasAttribute('data-no-transition')) return;
        if (e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
        if (!isSameOrigin(href)) return;

        e.preventDefault();
        navigate(href, true);
    });

    window.addEventListener('popstate', function () {
        navigate(window.location.href, false);
    });
})();