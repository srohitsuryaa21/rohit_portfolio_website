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

            document.querySelectorAll("[data-holo]").forEach(card => {
                card.addEventListener("pointermove", event => {
                    const rect = card.getBoundingClientRect();
                    const px = (event.clientX - rect.left) / rect.width;
                    const py = (event.clientY - rect.top) / rect.height;
                    card.style.setProperty("--ry", `${(px - .5) * 16}deg`);
                    card.style.setProperty("--rx", `${(.5 - py) * 16}deg`);
                    card.style.setProperty("--mx", `${20 + px * 60}%`);
                    card.style.setProperty("--my", `${20 + py * 60}%`);
                });
                card.addEventListener("pointerleave", () => {
                    card.style.setProperty("--rx", "0deg");
                    card.style.setProperty("--ry", "0deg");
                    card.style.setProperty("--mx", "50%");
                    card.style.setProperty("--my", "50%");
                });
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

        window.portfolio.initParticles();
    },

    // Dependency-free 3D "neural" particle field for the hero.
    // Real 3D points, rotation + perspective projection, connecting lines,
    // cursor parallax and interaction. No WebGL / no external library.
    initParticles: function () {
        const canvas = document.querySelector(".hero-particles");
        if (!canvas || canvas.dataset.on) return;
        if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
        if (window.innerWidth < 820) return;
        canvas.dataset.on = "1";

        const ctx = canvas.getContext("2d", { alpha: true });
        const hero = document.getElementById("hero") || canvas.parentElement;
        const SPREAD = 300, FOV = 640, LINK = 104, CURSOR_R = 165;
        let W = 0, H = 0, cx = 0, cy = 0, dpr = 1, points = [];

        function palette() {
            const dark = document.documentElement.getAttribute("data-theme") === "dark";
            return dark
                ? { r: 185, g: 255, b: 69, dot: 0.60, line: 0.28, cursor: 0.60 }
                : { r: 20, g: 63, b: 42, dot: 0.42, line: 0.18, cursor: 0.45 };
        }

        function build() {
            const count = Math.max(42, Math.min(105, Math.round((W * H) / 16500)));
            points = [];
            for (let i = 0; i < count; i++) {
                let x, y, z, d;
                do { x = Math.random() * 2 - 1; y = Math.random() * 2 - 1; z = Math.random() * 2 - 1; d = x * x + y * y + z * z; } while (d > 1);
                points.push({ x: x * SPREAD, y: y * SPREAD, z: z * SPREAD, ph: Math.random() * Math.PI * 2, sp: 0.6 + Math.random() * 0.9, sx: 0, sy: 0, s: 1, br: 1 });
            }
        }

        function resize() {
            dpr = Math.min(window.devicePixelRatio || 1, 2);
            const rect = hero.getBoundingClientRect();
            W = Math.max(1, rect.width); H = Math.max(1, rect.height);
            canvas.width = Math.round(W * dpr); canvas.height = Math.round(H * dpr);
            canvas.style.width = W + "px"; canvas.style.height = H + "px";
            ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
            cx = W / 2; cy = H / 2;
            build();
        }

        let mx = 0, my = 0, offX = 0, offY = 0, autoY = 0;
        let pointerX = -9999, pointerY = -9999, pointerIn = false;

        window.addEventListener("pointermove", e => {
            const rect = hero.getBoundingClientRect();
            pointerIn = e.clientX >= rect.left && e.clientX <= rect.right && e.clientY >= rect.top && e.clientY <= rect.bottom;
            pointerX = e.clientX - rect.left; pointerY = e.clientY - rect.top;
            mx = (pointerX / W - 0.5) * 2; my = (pointerY / H - 0.5) * 2;
        }, { passive: true });
        window.addEventListener("pointerleave", () => { pointerIn = false; pointerX = pointerY = -9999; mx = 0; my = 0; }, { passive: true });

        let visible = true, t0 = performance.now();

        function frame(now) {
            requestAnimationFrame(frame);
            if (!visible) return;
            const time = (now - t0) / 1000;

            offX += (-my * 0.4 - offX) * 0.05;
            offY += (mx * 0.55 - offY) * 0.05;
            autoY += 0.0022;
            const rotY = autoY + offY, rotX = offX;
            const cY = Math.cos(rotY), sY = Math.sin(rotY), cX = Math.cos(rotX), sX = Math.sin(rotX);
            const p = palette();

            ctx.clearRect(0, 0, W, H);

            for (let i = 0; i < points.length; i++) {
                const pt = points[i];
                const x1 = pt.x * cY - pt.z * sY;
                const z1 = pt.x * sY + pt.z * cY;
                const y2 = pt.y * cX - z1 * sX;
                const z2 = pt.y * sX + z1 * cX;
                const s = FOV / (FOV + z2);
                pt.sx = cx + x1 * s; pt.sy = cy + y2 * s; pt.s = s;
                pt.br = 0.65 + 0.35 * Math.sin(time * pt.sp + pt.ph);
            }

            ctx.lineWidth = 1;
            for (let i = 0; i < points.length; i++) {
                const a = points[i];
                for (let j = i + 1; j < points.length; j++) {
                    const b = points[j];
                    const dx = a.sx - b.sx, dy = a.sy - b.sy;
                    const d2 = dx * dx + dy * dy;
                    if (d2 > LINK * LINK) continue;
                    const d = Math.sqrt(d2);
                    const al = (1 - d / LINK) * p.line * ((a.s + b.s) * 0.5);
                    if (al < 0.02) continue;
                    ctx.strokeStyle = "rgba(" + p.r + "," + p.g + "," + p.b + "," + al + ")";
                    ctx.beginPath(); ctx.moveTo(a.sx, a.sy); ctx.lineTo(b.sx, b.sy); ctx.stroke();
                }
            }

            for (let i = 0; i < points.length; i++) {
                const pt = points[i];
                let boost = 0;
                if (pointerIn) {
                    const d = Math.hypot(pt.sx - pointerX, pt.sy - pointerY);
                    if (d < CURSOR_R) {
                        boost = 1 - d / CURSOR_R;
                        ctx.strokeStyle = "rgba(" + p.r + "," + p.g + "," + p.b + "," + (boost * p.cursor * pt.s) + ")";
                        ctx.beginPath(); ctx.moveTo(pt.sx, pt.sy); ctx.lineTo(pointerX, pointerY); ctx.stroke();
                    }
                }
                const rad = (1.1 + 1.7 * pt.s) * (1 + boost * 0.9);
                const al = Math.min(1, (0.35 + 0.65 * pt.s) * p.dot * pt.br + boost * 0.5);
                ctx.fillStyle = "rgba(" + p.r + "," + p.g + "," + p.b + "," + al + ")";
                ctx.beginPath(); ctx.arc(pt.sx, pt.sy, rad, 0, Math.PI * 2); ctx.fill();
            }
        }

        resize();
        window.addEventListener("resize", () => { clearTimeout(canvas._rt); canvas._rt = setTimeout(resize, 180); }, { passive: true });
        document.addEventListener("visibilitychange", () => { visible = !document.hidden; });
        if ("IntersectionObserver" in window) {
            new IntersectionObserver(es => es.forEach(e => visible = e.isIntersecting && !document.hidden), { threshold: 0 }).observe(hero);
        }
        requestAnimationFrame(frame);
        requestAnimationFrame(() => canvas.classList.add("is-live"));
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
