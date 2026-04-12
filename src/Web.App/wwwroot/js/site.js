// Theme toggle (dark/light mode)
(function () {
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const html = document.documentElement;

    window.mermaidNoop = window.mermaidNoop || function () {};

    // Load saved theme or default to dark
    const savedTheme = localStorage.getItem('theme') || 'dark';
    html.setAttribute('data-bs-theme', savedTheme);
    updateIcon(savedTheme);
    renderMermaid(savedTheme);

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const current = html.getAttribute('data-bs-theme');
            const next = current === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-bs-theme', next);
            localStorage.setItem('theme', next);
            updateIcon(next);
            renderMermaid(next);
        });
    }

    function updateIcon(theme) {
        if (themeIcon) {
            themeIcon.className = theme === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
        }

        if (themeToggle) {
            const nextTheme = theme === 'dark' ? 'light' : 'dark';
            themeToggle.setAttribute('aria-label', `Switch to ${nextTheme} theme`);
            themeToggle.setAttribute('title', `Switch to ${nextTheme} theme`);
        }
    }

    async function renderMermaid(theme) {
        if (!window.mermaid) {
            return;
        }

        const diagrams = document.querySelectorAll('.mermaid');
        if (diagrams.length === 0) {
            return;
        }

        diagrams.forEach((diagram) => {
            if (!diagram.dataset.mermaidSource) {
                diagram.dataset.mermaidSource = diagram.textContent.trim();
            }

            diagram.removeAttribute('data-processed');
            diagram.textContent = diagram.dataset.mermaidSource;
        });

        window.mermaid.initialize({
            startOnLoad: false,
            securityLevel: 'loose',
            theme: theme === 'dark' ? 'dark' : 'neutral',
            fontFamily: '"Manrope", "Segoe UI", sans-serif',
            themeCSS: theme === 'dark'
                ? `
.mermaidTooltip {
    display: block !important;
    width: max-content !important;
    max-width: min(26rem, calc(100vw - 2rem)) !important;
    padding: 0.55rem 0.7rem !important;
    border-radius: 0.75rem !important;
    border: 1px solid #33465a !important;
    background: rgba(12, 19, 28, 0.96) !important;
    color: #edf4fb !important;
    box-shadow: 0 18px 40px rgba(0, 0, 0, 0.35) !important;
    font-family: "Manrope", "Segoe UI", sans-serif !important;
    font-size: 0.78rem !important;
    line-height: 1.4 !important;
    text-align: left !important;
    white-space: normal !important;
    overflow-wrap: anywhere !important;
    word-break: break-word !important;
}
`
                : `
.mermaidTooltip {
    display: block !important;
    width: max-content !important;
    max-width: min(26rem, calc(100vw - 2rem)) !important;
    padding: 0.55rem 0.7rem !important;
    border-radius: 0.75rem !important;
    border: 1px solid #cad6e2 !important;
    background: rgba(255, 255, 255, 0.98) !important;
    color: #17212e !important;
    box-shadow: 0 18px 40px rgba(15, 23, 42, 0.12) !important;
    font-family: "Manrope", "Segoe UI", sans-serif !important;
    font-size: 0.78rem !important;
    line-height: 1.4 !important;
    text-align: left !important;
    white-space: normal !important;
    overflow-wrap: anywhere !important;
    word-break: break-word !important;
}
`,
            themeVariables: theme === 'dark'
                ? {
                    background: 'transparent',
                    fontSize: '12px',
                    primaryColor: '#142132',
                    primaryBorderColor: '#38516a',
                    primaryTextColor: '#edf4fb',
                    lineColor: '#8ea0b4',
                    actorBkg: '#142132',
                    actorBorder: '#38516a',
                    actorTextColor: '#edf4fb',
                    noteBkgColor: '#1a2a3d',
                    noteBorderColor: '#38516a'
                }
                : {
                    background: 'transparent',
                    fontSize: '12px',
                    primaryColor: '#f5f9fd',
                    primaryBorderColor: '#9db1c4',
                    primaryTextColor: '#17212e',
                    lineColor: '#5d7083',
                    actorBkg: '#f5f9fd',
                    actorBorder: '#9db1c4',
                    actorTextColor: '#17212e',
                    noteBkgColor: '#ecf3f9',
                    noteBorderColor: '#9db1c4'
                },
            sequence: {
                mirrorActors: false,
                wrap: true
            },
            flowchart: {
                useMaxWidth: true,
                curve: 'linear'
            }
        });

        try {
            await window.mermaid.run({
                nodes: Array.from(diagrams)
            });
        } catch (error) {
            console.error('Mermaid render failed', error);
        }
    }
})();
