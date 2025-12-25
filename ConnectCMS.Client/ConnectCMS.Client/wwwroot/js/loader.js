let baseColor = '#FFFFFF';
let primaryColor = '#F57650';
let elements = document.getElementsByClassName('cms-web');

for (let i = 0; i < elements.length; i++) {
    elements[i].style.backgroundImage = 'linear-gradient(-120deg,' + primaryColor + ' 50%,' + baseColor + ' 50%)';
}