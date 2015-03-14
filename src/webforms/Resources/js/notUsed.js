// NOT USED works with Chrome, NOT IE, Opera, buggy in FF

function animateYpos(target, y) {
    // create animation
    var animation = document.createElementNS(svgns, 'animate');
    animation.setAttributeNS(null, 'attributeName', 'y');
    animation.setAttributeNS(null, 'begin', 'indefinite');
    animation.setAttributeNS(null, 'to', y);
    animation.setAttributeNS(null, 'dur', 0.25);
    animation.setAttributeNS(null, 'fill', 'freeze');

    // link the animation to the target
    target.appendChild(animation);
    //document.getElementById("debugtext").appendChild(animation);

    // start the animation
    animation.beginElement();
}
