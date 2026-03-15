window.getCertificateHtml = (containerId) => {
    const element = document.getElementById(containerId);
    return element ? element.outerHTML : '';
};

window.convertToPdf = async function (elementId, filename) {
    const element = document.getElementById(elementId);

    // Pre-process images to handle CORS
    const processImages = async () => {
        const images = element.getElementsByTagName("img");
        const imagePromises = Array.from(images).map(async (img) => {
            // Skip if image is already a data URL
            if (img.src.startsWith('data:')) return;

            try {
                // Create a new image with crossOrigin attribute
                const newImg = new Image();
                newImg.crossOrigin = "anonymous";

                // Add required headers to the image URL
                const url = new URL(img.src);
                url.searchParams.append('timestamp', new Date().getTime());

                // Create canvas to convert image
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');

                // Wait for image to load
                await new Promise((resolve, reject) => {
                    newImg.onload = resolve;
                    newImg.onerror = reject;
                    newImg.src = url.toString();
                });

                // Draw image to canvas and convert to data URL
                canvas.width = newImg.width;
                canvas.height = newImg.height;
                ctx.drawImage(newImg, 0, 0);
                const dataUrl = canvas.toDataURL('image/png');

                // Replace an original image source with data URL
                img.src = dataUrl;
            } catch (error) {
                console.warn(`Failed to process image: ${img.src}`, error);
            }
        });

        await Promise.all(imagePromises);
    };

    // Configure html2canvas options
    const html2canvasOptions = {
        useCORS: true,
        allowTaint: true,
        logging: true,
        imageTimeout: 15000,
        scale: 2, // Increase quality
        onclone: function(clonedDoc) {
            // Additional processing on a cloned document if needed
            const clonedElement = clonedDoc.getElementById(elementId);
            // Ensure styles are properly applied in the clone
            if (clonedElement) {
                clonedElement.style.width = '100%';
                clonedElement.style.height = 'auto';
            }
        }
    };

    try {
        // Process images first
        await processImages();

        // Generate canvas
        const canvas = await html2canvas(element, html2canvasOptions);
        const imgData = canvas.toDataURL('image/png');

        // Create PDF
        const pdf = new jspdf.jsPDF({
            orientation: 'landscape',
            unit: 'mm'
        });

        const imgProps = pdf.getImageProperties(imgData);
        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
        pdf.save(filename);
    } catch (error) {
        console.error('Error generating PDF:', error);
        alert('There was an error generating the PDF. Please try again.');
    }
};