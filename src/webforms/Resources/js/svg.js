// svg.js
// quality check, run this script through http://www.jslint.com/
var SVGNS = "http://www.w3.org/2000/svg";
var XLINKNS = "http://www.w3.org/1999/xlink";
var SvgDoc; //evt.target.ownerDocument
var SvgInfoText; //user popup feedback info
var SvgDebugText; // debugging info
var SvgNoticeText; // label info
var root = document.documentElement;
var state = 'none', stateTarget, stateOrigin, stateTf;
var Angle = 0;
var ObjectsMovable = false;
var V_OFF = "visibility:hidden;";
var V_ON = "visibility:visible;";

// custom attributes for info 'injection' to the DOM
var _XV_C = "_xv_c"; //fs view, total child count
var _XV_VIRTUALY = "_xv_virtualy"; //fs view, remember relative y pos
var _XV_MODE = "_xv_mode"; //fs view, expanded, collapsed
var _XV_TYPE = "_xv_type"; //node, text, cdata, comment
var _XV_MSG = "_xv_msg"; //full text info storage
var _XV_SYMB = "_xv_symb"; // symbol notation for element type

function initApp(evt) {
    SvgDoc = evt.target.ownerDocument;
    SvgInfoText = document.getElementById("textbox").firstChild;
    SvgDebugText = document.getElementById("debugtext").firstChild;
    SvgNoticeText = document.getElementById("textNotice").firstChild;
}

// bottom values catch all because of browser difference behaviors
function isDefined(item) {
    return item !== null && item !== undefined && item !== "";
}

function removeChildrenFromNode(node) {
    if (isDefined(node)) {
        while (node.hasChildNodes()) {
            node.removeChild(node.firstChild);
        }
    }
}

// highlight
function HItem(evt) {
    var x = evt.clientX;
    var y = evt.clientY;
    if (false) { //doest quiet work good enough
        var rect = document.getElementById("rectTextbox");
        var textbox = document.getElementById("textbox");
        var flowtextarea = document.getElementById("flowtextarea");
        rect.setAttributeNS(null, "x", x + 160);
        rect.setAttributeNS(null, "y", y - 160);
        textbox.setAttributeNS(null, "x", x + 172);
        textbox.setAttributeNS(null, "y", y - 138);
        flowtextarea.setAttributeNS(null, "x", x + 172);
        flowtextarea.setAttributeNS(null, "y", y - 120);
    }

    evt.target.setAttribute("style", "font-size:30px;");
    SvgInfoText.nodeValue = "";
    document.getElementById("infobox").setAttributeNS(null, "class", "DisplayInline");
    var id = evt.target.getAttributeNS(null, "id");
    var type = evt.target.getAttributeNS(null, _XV_TYPE);
    var msg = evt.target.getAttributeNS(null, _XV_MSG);
    var str = "";
    if (isDefined(type)) { str = str + "type=" + type + " "; }
    // if (isDefined(id)){ str = str + "id=" + id + " ";}
    if (isDefined(msg)) { createTextFlows(evt); }

    SvgInfoText.nodeValue = str;
    if (evt.target.firstChild !== null) {
        SvgInfoText.nodeValue = SvgInfoText.nodeValue + "  " + evt.target.firstChild.nodeValue;        
    }
}

// unhighlight
function UItem(evt) {
    var textnode = document.getElementById('flowtextarea');
    textnode.nodeValue = "";
    removeChildrenFromNode(textnode); //remove tspan

    evt.target.setAttribute("style", "font-size:10px;");
    document.getElementById("infobox").setAttributeNS(null, "class", "DisplayNone");
}

//group
function g1(evt) {
    var element = evt.target;
    var attrib = evt.target.getAttributeNS(null, "id");
    if (isDefined(attrib)) {
        var id = "subg" + attrib;

        var elem = document.getElementById(id);
        var clas = elem.getAttributeNS(null, "class");
        if (clas === undefined || clas === "" || clas === "OpacityFull") {
            elem.setAttributeNS(null, "class", "OpacityLow");
        } else {
            elem.setAttributeNS(null, "class", "OpacityFull");
        }
        SvgNoticeText.nodeValue = id + " toggle";
    }
}
// alternative group fn
function g2(evt) {
    var element = evt.target;
    var attrib = evt.target.getAttributeNS(null, "id");
    if (isDefined(attrib)) {
        var id = "subg" + attrib;

        var elem = document.getElementById(id);
        var style = elem.getAttributeNS(null, "style");
        if (style === undefined || style === V_OFF) {
            elem.setAttributeNS(null, "style", "");
        } else {
            elem.setAttributeNS(null, "style", V_OFF);
        }
        SvgNoticeText.nodeValue = id + " toggle";
    }
}

// bigraph text lines toggle
function bgText(evt) {
    if (evt.target.firstChild !== null) {
        var element = evt.target;
        var attrib = evt.target.getAttributeNS(null, "id");
        if (isDefined(attrib)) {
            var id = "sub" + attrib;
            var elem = document.getElementById(id);
            var style = elem.getAttributeNS(null, "style");

            var v;
            if (!isDefined(style) || style === V_ON) {
                elem.setAttributeNS(null, "style", V_OFF);
                v = "off";
            } else {
                elem.setAttributeNS(null, "style", V_ON);
                v = "on";
            }
            SvgNoticeText.nodeValue = evt.target.firstChild.nodeValue + " toggle " + v;
        }
    }
}

// toggle all lines visible on, off
function bgRect(evt) {
    var elem = document.getElementById('bigraphLines');
    var style = elem.getAttributeNS(null, "style");
    var v;
    if (!isDefined(style) || style === V_ON) {
        elem.setAttributeNS(null, "style", V_OFF);
        v = "soft off";
    } else {
        elem.setAttributeNS(null, "style", "");
        v = "soft on";
    }
    SvgNoticeText.nodeValue = "Lines visible toggle " + v;
}

function min(a, b) {
    if (a <= b) {
        return a;
    }
    return b;
}
function max(a, b) {
    if (a >= b) {
        return a;
    }
    return b;
}

//----------------------------------
// Based on SVGPan 1.2
// changed: misc stuff and wheel event, now also supported by IE9+, Opera11+
function getCenterPoint() {
    var p = root.createSVGPoint();
    // center of 1024, 768
    p.x = 512;
    p.y = 384;
    return p;
}

function rotateView(angle) {
    //Angle = (Angle + angle) % 360;    
    var g = SvgDoc.getElementById("viewport");
    //stateTf = g.getCTM().inverse();
    //var p = getCenterPoint().matrixTransform(stateTf);
    //setCTM(g, stateTf.inverse().rotate(angle, 512, 284)); //method 1

    g.setAttributeNS(null, "transform", "rotate(" + angle + ",512,284)"); //method 2
    SvgNoticeText.nodeValue = "Rotate " + angle;
}

function setCTM(element, matrix) {
    var s = "matrix(" + matrix.a + "," + matrix.b + "," + matrix.c + "," + matrix.d + "," + matrix.e + "," + matrix.f + ")";
    element.setAttribute("transform", s);
}

function zoomy(level) {
    var z = level;
    var g = SvgDoc.getElementById("viewport");
    var p = getCenterPoint();
    p = p.matrixTransform(g.getCTM().inverse());

    // Compute new scale matrix in current mouse position
    var k = root.createSVGMatrix().translate(p.x, p.y).scale(z).translate(-p.x, -p.y);
    setCTM(g, g.getCTM().multiply(k));

    if (typeof (stateTf) === "undefined") {
        stateTf = g.getCTM().inverse();
    }
    stateTf = stateTf.multiply(k.inverse());
}

function zoomIn() {
    zoomy(1.1);
    SvgNoticeText.nodeValue = "+ Zoom";
}
function zoomOut() {
    zoomy(0.9);
    SvgNoticeText.nodeValue = "- Zoom";
}

function getEventPoint(evt) {
    var p = root.createSVGPoint();
    p.x = evt.clientX;
    p.y = evt.clientY;
    return p;
}

function getXYPoint(x, y) {
    var p = root.createSVGPoint();
    p.x = x;
    p.y = y;
    return p;
}

function setAttributes(element, attributes) {
    var a;
    for (a in attributes) {
        if (attributes.hasOwnProperty(a)) {
            element.setAttributeNS(null, a, attributes[a]);
        }
    }
}

function handleMouseWheel(evt) {
    if (evt.preventDefault) {
        evt.preventDefault();
    }
    evt.returnValue = false;
    var delta;
    if (evt.wheelDelta) {
        delta = evt.wheelDelta / 3600; // Chrome/Safari
    } else {
        delta = evt.detail / -90;  // Mozilla
    }
    var z = 1 + delta; // Zoom factor: 0.9/1.1
    //var SvgDoc = evt.target.ownerDocument;
    var g = SvgDoc.getElementById("viewport");
    var p = getEventPoint(evt);
    //var p = getCenterPoint();
    p = p.matrixTransform(g.getCTM().inverse());

    // Compute new scale matrix in current mouse position
    var k = root.createSVGMatrix().translate(p.x, p.y).scale(z).translate(-p.x, -p.y);
    setCTM(g, g.getCTM().multiply(k));

    if (typeof (stateTf) === "undefined") {
        stateTf = g.getCTM().inverse();
    }
    stateTf = stateTf.multiply(k.inverse());
}

function handleMouseMove(evt) {
    if (evt.preventDefault) {
        evt.preventDefault();
    }
    evt.returnValue = false;
    var g = SvgDoc.getElementById("viewport");
    if (state === 'pan') {
        // Pan mode
        var p = getEventPoint(evt).matrixTransform(stateTf);
        setCTM(g, stateTf.inverse().translate(p.x - stateOrigin.x, p.y - stateOrigin.y));
    } else if (ObjectsMovable && state === 'move') {
        // Move mode
        var pp = getEventPoint(evt).matrixTransform(g.getCTM().inverse());
        setCTM(stateTarget, root.createSVGMatrix().translate(pp.x - stateOrigin.x, pp.y - stateOrigin.y).multiply(g.getCTM().inverse()).multiply(stateTarget.getCTM()));
        stateOrigin = pp;
    }
}

function handleMouseDown(evt) {
    if (evt.preventDefault) {
        evt.preventDefault();
    }

    evt.returnValue = false;
    var g = SvgDoc.getElementById("viewport");
    if (evt.target.tagName === "svg") {
        // Pan mode
        state = 'pan';
        stateTf = g.getCTM().inverse();
        stateOrigin = getEventPoint(evt).matrixTransform(stateTf);
    } else {
        // Move mode
        state = 'move';
        stateTarget = evt.target;
        stateTf = g.getCTM().inverse();
        stateOrigin = getEventPoint(evt).matrixTransform(stateTf);
    }
}

function handleMouseUp(evt) {
    if (evt.preventDefault) {
        evt.preventDefault();
    }

    evt.returnValue = false;
    if (state === 'pan' || state === 'move') {
        // Quit pan mode
        state = '';
    }
}

function setupHandlers(root) {
    setAttributes(root, {
        "onmouseup": "add(evt)",
        "onmousedown": "handleMouseDown(evt)",
        "onmousemove": "handleMouseMove(evt)",
        "onmouseup": "handleMouseUp(evt)" //put , if below is used
        //"onmouseout" : "handleMouseUp(evt)" // Decomment this to stop the pan functionality when dragging out of the SVG element
    });

    /* Tested ok with Chrome 10+, Firefox 4+, Safari 5+, IE 9+, Opera 11+ */
    if (window.addEventListener) {
        /** DOMMouseScroll is for mozilla. */
        window.addEventListener('DOMMouseScroll', handleMouseWheel, false);
    }
    /** IE/Opera. */
    window.onmousewheel = document.onmousewheel = handleMouseWheel;
}
setupHandlers(root);
// end of SVGPan
//----------------------------------

/* textFlow.js */
//this function checks if there should be a dash at the given position, instead of a blank
function checkDashPosition(dashArray, pos) {
    var result = false;
    var i = 0;
    for (i = 0; i < dashArray.length; i++) {
        if (dashArray[i] === pos) {
            result = true;
        }
    }
    return result;
}

//Copyright (C) <2007>  <Andreas Neumann>
function textFlow(myText, textToAppend, maxWidth, x, ddy, justified) {
    //extract and add line breaks for start
    var dashArray = []; // new Array();
    var dashFound = true;
    var indexPos = 0;
    var cumulY = 0;
    while (dashFound === true) {
        var result = myText.indexOf("-", indexPos);
        if (result === -1) {
            //could not find a dash
            dashFound = false;
        }
        else {
            dashArray.push(result);
            indexPos = result + 1;
        }
    }
    //split the text at all spaces and dashes
    var words = myText.split(/[\s-]/);
    var line = "";
    var dy = 0;
    var curNumChars = 0;
    var computedTextLength = 0;
    var myTextNode;
    var tspanEl;
    var lastLineBreak = 0;

    for (i = 0; i < words.length; i++) {
        var word = words[i];
        curNumChars += word.length + 1;
        if (computedTextLength > maxWidth || i === 0) {
            if (computedTextLength > maxWidth) {
                var tempText = tspanEl.firstChild.nodeValue;
                tempText = tempText.slice(0, (tempText.length - words[i - 1].length - 2)); //the -2 is because we also strip off white space
                tspanEl.firstChild.nodeValue = tempText;
                if (justified) {
                    //determine the number of words in this line
                    var nrWords = tempText.split(/\s/).length;
                    computedTextLength = tspanEl.getComputedTextLength();
                    var additionalWordSpacing = (maxWidth - computedTextLength) / (nrWords - 1);
                    tspanEl.setAttributeNS(null, "word-spacing", additionalWordSpacing);
                    //alternatively one could use textLength and lengthAdjust, however, currently this is not too well supported in SVG UA's
                }
            }
            tspanEl = document.createElementNS(SVGNS, "tspan");
            tspanEl.setAttributeNS(null, "x", x);
            tspanEl.setAttributeNS(null, "dy", dy);
            myTextNode = document.createTextNode(line);
            tspanEl.appendChild(myTextNode);
            textToAppend.appendChild(tspanEl);

            if (checkDashPosition(dashArray, curNumChars - 1)) {
                line = word + "-";
            }
            else {
                line = word + " ";
            }
            if (i !== 0) {
                line = words[i - 1] + " " + line;
            }
            dy = ddy;
            cumulY += dy;
        }
        else {
            if (checkDashPosition(dashArray, curNumChars - 1)) {
                line += word + "-";
            }
            else {
                line += word + " ";
            }
        }
        tspanEl.firstChild.nodeValue = line;
        computedTextLength = tspanEl.getComputedTextLength();
        if (i === words.length - 1) {
            if (computedTextLength > maxWidth) {
                var tempText = tspanEl.firstChild.nodeValue;
                tspanEl.firstChild.nodeValue = tempText.slice(0, (tempText.length - words[i].length - 1));
                tspanEl = document.createElementNS(SVGNS, "tspan");
                tspanEl.setAttributeNS(null, "x", x);
                tspanEl.setAttributeNS(null, "dy", dy);
                myTextNode = document.createTextNode(words[i]);
                tspanEl.appendChild(myTextNode);
                textToAppend.appendChild(tspanEl);
            }

        }
    }
    return cumulY;
}

function createTextFlows(evt) {
    var msg = evt.target.getAttributeNS(null, _XV_MSG);
    if (isDefined(msg)) {
        var textnode = document.getElementById('flowtextarea');
        textnode.nodeValue = "";
        removeChildrenFromNode(textnode);
        var dy = textFlow(msg, textnode, 500, 392, 15, false); //myText,textnode,maxWidth,x,ddy,justified
    }
}

