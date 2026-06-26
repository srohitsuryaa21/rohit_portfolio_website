window.portfolio = {
    isDark: function () {
        return document.documentElement.getAttribute("data-theme") === "dark";
    },
    setTheme: function (isDark) {
        if (isDark) {
            document.documentElement.setAttribute("data-theme", "dark");
        } else {
            document.documentElement.removeAttribute("data-theme");
        }
        localStorage.setItem("portfolio-theme", isDark ? "dark" : "light");
    },
    initExperience: function () {
        const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        const progress = document.querySelector(".scroll-progress span");
        const glow = document.querySelector(".cursor-glow");

        const updateScroll = () => {
            const max = document.documentElement.scrollHeight - window.innerHeight;
            if (progress) progress.style.width = `${max > 0 ? (window.scrollY / max) * 100 : 0}%`;
        };
        window.addEventListener("scroll", updateScroll, { passive: true });
        updateScroll();

        if (glow && !reducedMotion) {
            window.addEventListener("pointermove", event => {
                glow.style.left = `${event.clientX}px`;
                glow.style.top = `${event.clientY}px`;
            }, { passive: true });
        }

        const reveals = document.querySelectorAll(".reveal");
        if (reducedMotion) {
            reveals.forEach(element => element.classList.add("visible"));
        } else {
            const revealObserver = new IntersectionObserver(entries => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add("visible");
                        revealObserver.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.12, rootMargin: "0px 0px -40px" });
            reveals.forEach(element => revealObserver.observe(element));
        }

        const navLinks = [...document.querySelectorAll("[data-nav]")];
        const sections = document.querySelectorAll("[data-section]");
        const sectionObserver = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;
                navLinks.forEach(link => link.classList.toggle("active", link.dataset.nav === entry.target.id));
            });
        }, { rootMargin: "-35% 0px -55%", threshold: 0 });
        sections.forEach(section => sectionObserver.observe(section));

        document.querySelectorAll(".counter").forEach(counter => {
            const target = Number(counter.dataset.count || 0);
            if (reducedMotion) {
                counter.textContent = target;
                return;
            }
            let current = 0;
            const timer = window.setInterval(() => {
                current += Math.max(1, Math.ceil((target - current) / 9));
                counter.textContent = Math.min(current, target);
                if (current >= target) window.clearInterval(timer);
            }, 32);
        });

        if (!reducedMotion && window.matchMedia("(pointer: fine)").matches) {
            document.querySelectorAll("[data-tilt]").forEach(card => {
                card.addEventListener("pointermove", event => {
                    const rect = card.getBoundingClientRect();
                    const x = (event.clientX - rect.left) / rect.width - .5;
                    const y = (event.clientY - rect.top) / rect.height - .5;
                    card.style.transform = `perspective(1100px) rotateX(${y * -3}deg) rotateY(${x * 4}deg) translateY(-3px)`;
                });
                card.addEventListener("pointerleave", () => card.style.transform = "");
            });

            document.querySelectorAll(".magnetic").forEach(element => {
                element.addEventListener("pointermove", event => {
                    const rect = element.getBoundingClientRect();
                    element.style.transform = `translate(${(event.clientX - rect.left - rect.width / 2) * .08}px, ${(event.clientY - rect.top - rect.height / 2) * .12}px)`;
                });
                element.addEventListener("pointerleave", () => element.style.transform = "");
            });
        }

        window.addEventListener("keydown", event => {
            if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k") {
                event.preventDefault();
                document.querySelector(".command-trigger")?.click();
            }
            if (document.activeElement?.matches("[data-project-stack]") && (event.key === "ArrowLeft" || event.key === "ArrowRight")) {
                event.preventDefault();
                const controls = document.querySelectorAll(".project-stack-controls button");
                controls[event.key === "ArrowRight" ? 1 : 0]?.click();
            }
            if (event.key === "Escape") {
                document.querySelector(".modal-close")?.click();
            }
        });
    }
};

(function () {
    const savedTheme = localStorage.getItem("portfolio-theme");
    if (savedTheme !== "light") {
        document.documentElement.setAttribute("data-theme", "dark");
    } else {
        document.documentElement.removeAttribute("data-theme");
    }
})();
