function downloadFile(data, fileName, mimeType) {
    const blob = new Blob([data], {type: mimeType});
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
}

function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function() {
        console.log('Text copied to clipboard');
    }).catch(function(error) {
        console.error('Error copying text: ', error);
    });
}

function enableCopyProtection() {
    // Disable right click
    document.addEventListener('contextmenu', function(e) {
        e.preventDefault();
        return false;
    });

    // Disable Ctrl+C, Ctrl+X, Ctrl+V, Ctrl+S, Ctrl+U, Ctrl+P, Ctrl+Shift+I, F12
    document.addEventListener('keydown', function(e) {
        if (
            // Prevent Ctrl + Shift + I
            (e.ctrlKey && e.shiftKey && e.keyCode === 73) ||
            // Prevent Ctrl + Shift + J
            (e.ctrlKey && e.shiftKey && e.keyCode === 74) ||
            // Prevent Ctrl + U
            (e.ctrlKey && e.keyCode === 85) ||
            // Prevent F12
            (e.keyCode === 123) ||
            // Prevent other Ctrl combinations
            (e.ctrlKey &&
                (e.keyCode === 67 || // C key
                    e.keyCode === 86 || // V key
                    e.keyCode === 83 || // S key
                    e.keyCode === 80 || // P key
                    e.keyCode === 88))  // X key
        ) {
            e.preventDefault();
            return false;
        }
    });

    // Disable print screen
    document.addEventListener('keyup', function(e) {
        if (e.keyCode === 44) { // Print Screen key
            e.preventDefault();
            return false;
        }
    });

    // Disable selecting text
    document.addEventListener('selectstart', function(e) {
        e.preventDefault();
        return false;
    });

    // Disable drag and drop
    document.addEventListener('dragstart', function(e) {
        e.preventDefault();
        return false;
    });

    // Add CSS to prevent text selection
    const style = document.createElement('style');
    style.textContent = `
        body {
            -webkit-user-select: none;
            -moz-user-select: none;
            -ms-user-select: none;
            user-select: none;
        }
    `;
    document.head.appendChild(style);

    // Disable developer tools through devtools event
    document.addEventListener('devtoolschange', function(e) {
        if(e.detail.isOpen) {
            window.location.reload();
        }
    });

    // Additional protection against inspect element
    setInterval(function() {
        const heightDiff = window.outerHeight - window.innerHeight;
        const widthDiff = window.outerWidth - window.innerWidth;

        // If devtools is open, typically the difference will be larger
        if (heightDiff > 100 || widthDiff > 100) {
            document.body.innerHTML = 'Developer tools are not allowed on this page.';
        }
    }, 1000);

    // Disable debugging
    function debug() {
        debugger;
        return debug;
    }
    debug();
}