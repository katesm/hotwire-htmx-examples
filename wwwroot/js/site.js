import { Application, Controller } from "https://cdn.jsdelivr.net/npm/@hotwired/stimulus@3.2.2/+esm";

class CounterController extends Controller {
    static targets = ["count"];

    increment() {
        this.countTarget.value = Number(this.countTarget.value) + 1;
    }

    decrement() {
        this.countTarget.value = Number(this.countTarget.value) - 1;
    }

    reset() {
        this.countTarget.value = 0;
    }
}

const application = Application.start();
application.register("counter", CounterController);

function getModalElements() {
    return {
        modal: document.getElementById("modal"),
        modalFrame: document.getElementById("modal-content")
    };
}

function closeModal({ clearFrame = false } = {}) {
    const { modal, modalFrame } = getModalElements();
    if (!modal) return;

    if (modal.open) modal.close();
    if (clearFrame && modalFrame) modalFrame.innerHTML = "";
}

function syncModalVisibility({ forceOpen = false } = {}) {
    const { modal, modalFrame } = getModalElements();
    if (!modal || !modalFrame) return;

    const hasContent = modalFrame.innerHTML.trim() !== "";

    if (forceOpen && hasContent && !modal.open) {
        modal.showModal();
        return;
    }

    if (!hasContent && modal.open) {
        modal.close();
    }
}

function ensureEmployeeStatsTarget() {
    if (!location.pathname.toLowerCase().startsWith('/employees')) return;
    if (document.getElementById('employee-stats-container')) return;

    const labels = Array.from(document.querySelectorAll('p'));
    const totalLabel = labels.find((label) => label.textContent?.trim() === 'Total Employees');
    const card = totalLabel?.parentElement;
    const container = card?.parentElement;

    if (container && !container.id) {
        container.id = 'employee-stats-container';
    }
}

let modalFrameObserver;

function attachModalFrameObserver() {
    if (modalFrameObserver) {
        modalFrameObserver.disconnect();
        modalFrameObserver = undefined;
    }

    const { modalFrame } = getModalElements();
    if (!modalFrame) return;

    modalFrameObserver = new MutationObserver(() => {
        syncModalVisibility();
    });

    modalFrameObserver.observe(modalFrame, {
        childList: true,
        subtree: true,
        characterData: true
    });
}


// document survives Turbo Drive navigation; document.body does not
// code is here to re-process hx-* attributes after Turbo Drive replaces the body
document.addEventListener("htmx:afterSwap", (e) => {
    if (e.detail.target.id === "modal-content") {
        syncModalVisibility({ forceOpen: true });
    }
});

document.addEventListener("closeModal", () => {
    closeModal({ clearFrame: true });
});

document.addEventListener("turbo:frame-load", (event) => {
    if (event.target.id !== "modal-content") return;
    attachModalFrameObserver();
    syncModalVisibility({ forceOpen: true });
});

document.addEventListener("turbo:before-stream-render", (event) => {
    const stream = event.target;
    if (!(stream instanceof Element)) return;

    if (stream.getAttribute("target") !== "modal-content") return;

    requestAnimationFrame(() => syncModalVisibility());
});

// Prevent stale dialog/backdrop from being cached by Turbo Drive.
document.addEventListener("turbo:before-cache", () => {
    closeModal({ clearFrame: true });
});

document.addEventListener("turbo:submit-end", (event) => {
    const form = event.target;
    if (!(form instanceof HTMLFormElement)) return;
    if (!form.closest("#modal-content")) return;

    requestAnimationFrame(() => syncModalVisibility());
});

// Re-process hx-* attributes after Turbo Drive replaces the body
document.addEventListener("turbo:load", () => {
    ensureEmployeeStatsTarget();
    attachModalFrameObserver();
    syncModalVisibility();
    if (typeof htmx !== "undefined") htmx.process(document.body);
});
