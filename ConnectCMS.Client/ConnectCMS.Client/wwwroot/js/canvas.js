window.loadHtml2Canvas = function () {
};

window.downloadCertificateAsImage = function () {
    const certificateElement = document.getElementById('certificate-container');

    html2canvas(certificateElement, {
        scale: 2,
        useCORS: true,
        backgroundColor: '#d3cfcf'
    }).then(function(canvas) {
        const imageData = canvas.toDataURL('image/png');

        const link = document.createElement('a');
        link.download = 'certificate.png';
        link.href = imageData;

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });
};