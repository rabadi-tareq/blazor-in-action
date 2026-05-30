// JavaScript module for CycleOfLife component
// Demonstrates JS interop best practices in Blazor lifecycle

let dotNetReference = null;

export function initialize(dotNetRef) {
    dotNetReference = dotNetRef;
    console.log('CycleOfLife component initialized from JavaScript');

    // Example: Set up event listeners or initialize JS libraries
    setupEventListeners();
}

function setupEventListeners() {
    // Example: Listen to window resize, scroll, etc.
    // This demonstrates how you might interact with the DOM after first render
}

export function dispose() {
    console.log('CycleOfLife component disposing from JavaScript');

    // Clean up event listeners
    dotNetReference = null;
}

// Example function that could be called from .NET
export function someJavaScriptFunction(data) {
    console.log('Called from .NET:', data);

    // Call back to .NET
    if (dotNetReference) {
        dotNetReference.invokeMethodAsync('OnJavaScriptCallback', 'Hello from JS!');
    }
}
