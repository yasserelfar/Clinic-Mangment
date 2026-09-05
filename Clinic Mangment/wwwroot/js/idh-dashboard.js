window.IDHDashboard = {
    init: function () {
        var reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

        // ---------- 1) Count-up numbers ----------
        var countTargets = document.querySelectorAll('.idh-stat-value, .card-body h2');

        countTargets.forEach(function (el, i) {
            var target = parseInt(el.textContent.trim(), 10);
            if (isNaN(target)) return;

            if (reduceMotion) {
                el.textContent = target;
                return;
            }

            var duration = 900;
            var start = null;
            el.textContent = '0';

            function step(timestamp) {
                if (!start) start = timestamp;
                var progress = Math.min((timestamp - start) / duration, 1);
                var eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = Math.floor(eased * target);
                if (progress < 1) {
                    requestAnimationFrame(step);
                } else {
                    el.textContent = target;
                }
            }

            setTimeout(function () {
                requestAnimationFrame(step);
            }, 150 + i * 80);
        });

        if (reduceMotion) return;

        // ---------- 2) Spotlight hover glow ----------
        var glowTargets = document.querySelectorAll('.idh-stat-card, .card.shadow-sm');
        glowTargets.forEach(function (card) {
            card.addEventListener('mousemove', function (e) {
                var rect = card.getBoundingClientRect();
                card.style.setProperty('--mx', (e.clientX - rect.left) + 'px');
                card.style.setProperty('--my', (e.clientY - rect.top) + 'px');
            });
        });

        // ---------- 3) Ripple click effect ----------
        var rippleTargets = document.querySelectorAll(
            '.idh-add-btn, .idh-open-btn, .idh-btn-secondary, .idh-btn-xs, .idh-submit-btn.btn'
        );
        rippleTargets.forEach(function (btn) {
            var computed = window.getComputedStyle(btn);
            if (computed.position === 'static') btn.style.position = 'relative';
            btn.style.overflow = 'hidden';

            btn.addEventListener('click', function (e) {
                var rect = btn.getBoundingClientRect();
                var size = Math.max(rect.width, rect.height);
                var ripple = document.createElement('span');
                ripple.className = 'idh-ripple';
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = (e.clientX - rect.left - size / 2) + 'px';
                ripple.style.top = (e.clientY - rect.top - size / 2) + 'px';
                btn.appendChild(ripple);
                setTimeout(function () { ripple.remove(); }, 600);
            });
        });

        // ---------- 4) Scroll reveal for below-the-fold panels ----------
        var revealTargets = document.querySelectorAll(
            '.idh-panel:nth-of-type(2), .card.shadow-sm.mt-5'
        );
        revealTargets.forEach(function (el) { el.classList.add('idh-scroll-reveal'); });

        if ('IntersectionObserver' in window) {
            var observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('idh-inview');
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.15 });

            revealTargets.forEach(function (el) { observer.observe(el); });
        } else {
            revealTargets.forEach(function (el) { el.classList.add('idh-inview'); });
        }
    }
};

document.addEventListener("DOMContentLoaded", function () {
    IDHDashboard.init();
    IDHTimeline.init();
});
function initTimeline() {
    var timelineItems = document.querySelectorAll('.timeline-item');

    timelineItems.forEach(function (item, index) {
        item.style.animationDelay = (0.15 + index * 0.15) + 's';

        var dot = item.querySelector('.timeline-dot');

        if (dot) {
            dot.style.animationDelay = (0.35 + index * 0.15) + 's';
        }
    });
}
window.IDHTimeline = {

    init: function () {

        const timeline = document.querySelector(".timeline");
        const items = document.querySelectorAll(".timeline-item");

        if (!timeline || !items.length) return;

        const observer = new IntersectionObserver(
            (entries) => {

                entries.forEach(entry => {

                    if (entry.isIntersecting) {

                        // دخل الشاشة
                        entry.target.classList.add("show");

                    } else {

                        // خرج من الشاشة
                        entry.target.classList.remove("show");

                    }

                });

            },
            {
                threshold: 0.15,
                rootMargin: "0px 0px -50px 0px"
            }
        );

        items.forEach(item => {
            observer.observe(item);
        });
    }
};