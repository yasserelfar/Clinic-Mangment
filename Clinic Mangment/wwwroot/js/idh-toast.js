window.IDHToast = (function () {
    let container = null;

    function ensureContainer() {
        if (container && document.body.contains(container)) return container;
        container = document.createElement("div");
        container.id = "idh-toast-container";
        document.body.appendChild(container);
        return container;
    }

    function show(message, type) {
        type = type || "success";
        const el = ensureContainer();

        const toast = document.createElement("div");
        toast.className = "idh-toast idh-toast-" + type;

        const icon = document.createElement("span");
        icon.className = "idh-toast-icon";

        const text = document.createElement("span");
        text.className = "idh-toast-text";
        text.textContent = message;

        const closeBtn = document.createElement("button");
        closeBtn.className = "idh-toast-close";
        closeBtn.setAttribute("aria-label", "Dismiss");
        closeBtn.textContent = "×";

        toast.appendChild(icon);
        toast.appendChild(text);
        toast.appendChild(closeBtn);
        el.appendChild(toast);

        requestAnimationFrame(function () {
            toast.classList.add("idh-toast-in");
        });

        function dismiss() {
            toast.classList.remove("idh-toast-in");
            toast.classList.add("idh-toast-out");
            setTimeout(function () { toast.remove(); }, 250);
        }

        closeBtn.addEventListener("click", dismiss);
        const timer = setTimeout(dismiss, 4000);
        toast.addEventListener("mouseenter", function () { clearTimeout(timer); });
    }

    return { show: show };
})();