/* textflow usages */
function createTextFlows() {
    var myFlowText = "SVG is a language for describing two-dimensional graphics and graphical applications in XML. SVG 1.1 is a W3C Recommendation and forms the core of the current SVG developments. SVG 1.2 is the specification currently being developed as is available in draft form (comments welcome). The SVG Mobile Profiles: SVG Basic and SVG Tiny are targeted to resource-limited devices and are part of the 3GPP platform for third generation mobile phones. SVG Print is a set of guidelines to produce final-form documents in XML suitable for archiving and printing.";
    var textNodeJustified = document.getElementById('flowtextarea');

    textNodeJustified.nodeValue = " ";
    var dy = textFlow(myFlowText, textNodeJustified, 400, 300, 15, true);
}

/* helper_functions.js */
//@author Andreas Neumann a.neumann@carto.net
//@copyright LGPL 2.1 <a href="http://www.gnu.org/copyleft/lesser.txt">Gnu LGPL 2.1</a>
var svgNS = "http://www.w3.org/2000/svg";
var xlinkNS = "http://www.w3.org/1999/xlink";
var cartoNS = "http://www.carto.net/attrib";
var attribNS = "http://www.carto.net/attrib";
var batikNS = "http://xml.apache.org/batik/ext";

function toPolarDir(xdiff, ydiff) {
    var direction = (Math.atan2(ydiff, xdiff));
    return (direction);
}

function toPolarDist(xdiff, ydiff) {
    var distance = Math.sqrt(xdiff * xdiff + ydiff * ydiff);
    return (distance);
}

function toRectX(direction, distance) {
    var x = distance * Math.cos(direction);
    return (x);
}

function toRectY(direction, distance) {
    y = distance * Math.sin(direction);
    return (y);
}

function DegToRad(deg) {
    return (deg / 180.0 * Math.PI);
}

function RadToDeg(rad) {
    return (rad / Math.PI * 180.0);
}

function dd2dms(dd) {
    var minutes = (Math.abs(dd) - Math.floor(Math.abs(dd))) * 60;
    var seconds = (minutes - Math.floor(minutes)) * 60;
    var minutes = Math.floor(minutes);
    if (dd >= 0) {
        var degrees = Math.floor(dd);
    }
    else {
        var degrees = Math.ceil(dd);
    }
    return { deg: degrees, min: minutes, sec: seconds };
}

function dms2dd(deg, min, sec) {
    if (deg < 0) {
        return deg - (min / 60) - (sec / 3600);
    }
    else {
        return deg + (min / 60) + (sec / 3600);
    }
}

function log(x, b) {
    if (b == null) b = Math.E;
    return Math.log(x) / Math.log(b);
}

function intBilinear(za, zb, zc, zd, xpos, ypos, ax, ay, cellsize) { //bilinear interpolation function
    var e = (xpos - ax) / cellsize;
    var f = (ypos - ay) / cellsize;

    //calculation of weights
    var wa = (1 - e) * (1 - f);
    var wb = e * (1 - f);
    var wc = e * f;
    var wd = f * (1 - e);

    var interpol_value = wa * zc + wb * zd + wc * za + wd * zb;
    return interpol_value;
}

function leftOfTest(pointx, pointy, linex1, liney1, linex2, liney2) {
    var result = (liney1 - pointy) * (linex2 - linex1) - (linex1 - pointx) * (liney2 - liney1);
    if (result < 0) {
        var leftof = 1; //case left of
    }
    else {
        var leftof = 0; //case left of	
    }
    return leftof;
}

function distFromLine(xpoint, ypoint, linex1, liney1, linex2, liney2) {
    var dx = linex2 - linex1;
    var dy = liney2 - liney1;
    var distance = (dy * (xpoint - linex1) - dx * (ypoint - liney1)) / Math.sqrt(Math.pow(dx, 2) + Math.pow(dy, 2));
    return distance;
}

function angleBetwTwoLines(ax, ay, bx, by) {
    var angle = Math.acos((ax * bx + ay * by) / (Math.sqrt(Math.pow(ax, 2) + Math.pow(ay, 2)) * Math.sqrt(Math.pow(bx, 2) + Math.pow(by, 2))));
    return angle;
}

function calcBisectorVector(ax, ay, bx, by) {
    var betraga = Math.sqrt(Math.pow(ax, 2) + Math.pow(ay, 2));
    var betragb = Math.sqrt(Math.pow(bx, 2) + Math.pow(by, 2));
    var c = new Array();
    c[0] = ax / betraga + bx / betragb;
    c[1] = ay / betraga + by / betragb;
    return c;
}

function calcBisectorAngle(ax, ay, bx, by) {
    var betraga = Math.sqrt(Math.pow(ax, 2) + Math.pow(ay, 2));
    var betragb = Math.sqrt(Math.pow(bx, 2) + Math.pow(by, 2));
    var c1 = ax / betraga + bx / betragb;
    var c2 = ay / betraga + by / betragb;
    var angle = toPolarDir(c1, c2);
    return angle;
}

function intersect2lines(line1x1, line1y1, line1x2, line1y2, line2x1, line2y1, line2x2, line2y2) {
    var interSectPoint = new Object();
    var denominator = (line2y2 - line2y1) * (line1x2 - line1x1) - (line2x2 - line2x1) * (line1y2 - line1y1);
    if (denominator == 0) {
        alert("lines are parallel");
    }
    else {
        var ua = ((line2x2 - line2x1) * (line1y1 - line2y1) - (line2y2 - line2y1) * (line1x1 - line2x1)) / denominator;
        var ub = ((line1x2 - line1x1) * (line1y1 - line2y1) - (line1y2 - line1y1) * (line1x1 - line2x1)) / denominator;
    }
    interSectPoint["x"] = line1x1 + ua * (line1x2 - line1x1);
    interSectPoint["y"] = line1y1 + ua * (line1y2 - line1y1);
    return interSectPoint;
}

function formatNumberString(inputNumber, separator) {
    //check if of type string, if number, convert it to string
    if (typeof (inputNumber) == "Number") {
        var myTempString = inputNumber.toString();
    }
    else {
        var myTempString = inputNumber;
    }
    var newString = "";
    //if it contains a comma, it will be split
    var splitResults = myTempString.split(".");
    var myCounter = splitResults[0].length;
    if (myCounter > 3) {
        while (myCounter > 0) {
            if (myCounter > 3) {
                newString = separator + splitResults[0].substr(myCounter - 3, 3) + newString;
            }
            else {
                newString = splitResults[0].substr(0, myCounter) + newString;
            }
            myCounter -= 3;
        }
    }
    else {
        newString = splitResults[0];
    }
    //concatenate if it contains a comma
    if (splitResults[1]) {
        newString = newString + "." + splitResults[1];
    }
    return newString;
}

function statusChange(statusText) {
    document.getElementById("statusText").firstChild.nodeValue = "Statusbar: " + statusText;
}


function scaleObject(evt, factor) {
    //reference to the currently selected object
    var element = evt.currentTarget;
    var myX = element.getAttributeNS(null, "x");
    var myY = element.getAttributeNS(null, "y");
    var newtransform = "scale(" + factor + ") translate(" + (myX * 1 / factor - myX) + " " + (myY * 1 / factor - myY) + ")";
    element.setAttributeNS(null, 'transform', newtransform);
}


function getTransformToRootElement(node) {
    try {
        //this part is for fully conformant players (like Opera, Batik, Firefox, Safari ...)
        var CTM = node.getTransformToElement(document.documentElement);
    }
    catch (ex) {
        //this part is for ASV3 or other non-conformant players
        // Initialize our CTM the node's Current Transformation Matrix
        var CTM = node.getCTM();
        // Work our way through the ancestor nodes stopping at the SVG Document
        while ((node = node.parentNode) != document) {
            // Multiply the new CTM to the one with what we have accumulated so far
            CTM = node.getCTM().multiply(CTM);
        }
    }
    return CTM;
}

function getTransformToElement(node, targetNode) {
    try {
        //this part is for fully conformant players
        var CTM = node.getTransformToElement(targetNode);
    }
    catch (ex) {
        //this part is for ASV3 or other non-conformant players
        // Initialize our CTM the node's Current Transformation Matrix
        var CTM = node.getCTM();
        // Work our way through the ancestor nodes stopping at the SVG Document
        while ((node = node.parentNode) != targetNode) {
            // Multiply the new CTM to the one with what we have accumulated so far
            CTM = node.getCTM().multiply(CTM);
        }
    }
    return CTM;
}

function hsv2rgb(hue, sat, val) {
    var rgbArr = new Object();
    if (sat == 0) {
        rgbArr["red"] = Math.round(val * 255);
        rgbArr["green"] = Math.round(val * 255);
        rgbArr["blue"] = Math.round(val * 255);
    }
    else {
        var h = hue / 60;
        var i = Math.floor(h);
        var f = h - i;
        if (i % 2 == 0) {
            f = 1 - f;
        }
        var m = val * (1 - sat);
        var n = val * (1 - sat * f);
        switch (i) {
            case 0:
                rgbArr["red"] = val;
                rgbArr["green"] = n;
                rgbArr["blue"] = m;
                break;
            case 1:
                rgbArr["red"] = n;
                rgbArr["green"] = val;
                rgbArr["blue"] = m;
                break;
            case 2:
                rgbArr["red"] = m;
                rgbArr["green"] = val;
                rgbArr["blue"] = n;
                break;
            case 3:
                rgbArr["red"] = m;
                rgbArr["green"] = n;
                rgbArr["blue"] = val;
                break;
            case 4:
                rgbArr["red"] = n;
                rgbArr["green"] = m;
                rgbArr["blue"] = val;
                break;
            case 5:
                rgbArr["red"] = val;
                rgbArr["green"] = m;
                rgbArr["blue"] = n;
                break;
            case 6:
                rgbArr["red"] = val;
                rgbArr["green"] = n;
                rgbArr["blue"] = m;
                break;
        }
        rgbArr["red"] = Math.round(rgbArr["red"] * 255);
        rgbArr["green"] = Math.round(rgbArr["green"] * 255);
        rgbArr["blue"] = Math.round(rgbArr["blue"] * 255);
    }
    return rgbArr;
}

function rgb2hsv(red, green, blue) {
    var hsvArr = new Object();
    red = red / 255;
    green = green / 255;
    blue = blue / 255;
    myMax = Math.max(red, Math.max(green, blue));
    myMin = Math.min(red, Math.min(green, blue));
    v = myMax;
    if (myMax > 0) {
        s = (myMax - myMin) / myMax;
    }
    else {
        s = 0;
    }
    if (s > 0) {
        myDiff = myMax - myMin;
        rc = (myMax - red) / myDiff;
        gc = (myMax - green) / myDiff;
        bc = (myMax - blue) / myDiff;
        if (red == myMax) {
            h = (bc - gc) / 6;
        }
        if (green == myMax) {
            h = (2 + rc - bc) / 6;
        }
        if (blue == myMax) {
            h = (4 + gc - rc) / 6;
        }
    }
    else {
        h = 0;
    }
    if (h < 0) {
        h += 1;
    }
    hsvArr["hue"] = Math.round(h * 360);
    hsvArr["sat"] = s;
    hsvArr["val"] = v;
    return hsvArr;
}

function arrayPopulate(arrayKeys, arrayValues) {
    var returnArray = new Array();
    if (arrayKeys.length != arrayValues.length) {
        alert("error: arrays do not have the same length!");
    }
    else {
        for (i = 0; i < arrayKeys.length; i++) {
            returnArray[arrayKeys[i]] = arrayValues[i];
        }
    }
    return returnArray;
}


function getData(url, callBackFunction, returnFormat, method, postText, additionalParams) {
    this.url = url;
    this.callBackFunction = callBackFunction;
    this.returnFormat = returnFormat;
    this.method = method;
    this.additionalParams = additionalParams;
    if (method != "get" && method != "post") {
        alert("Error in network request: parameter 'method' must be 'get' or 'post'");
    }
    this.postText = postText;
    this.xmlRequest = null; //@private reference to the XMLHttpRequest object
}


getData.prototype.getData = function () {
    //call getURL() if available
    if (window.getURL) {
        if (this.method == "get") {
            getURL(this.url, this);
        }
        if (this.method == "post") {
            postURL(this.url, this.postText, this);
        }
    }
    //or call XMLHttpRequest() if available
    else if (window.XMLHttpRequest) {
        var _this = this;
        this.xmlRequest = new XMLHttpRequest();
        if (this.method == "get") {
            if (this.returnFormat == "xml") {
                this.xmlRequest.overrideMimeType("text/xml");
            }
            this.xmlRequest.open("GET", this.url, true);
        }
        if (this.method == "post") {
            this.xmlRequest.open("POST", this.url, true);
        }
        this.xmlRequest.onreadystatechange = function () { _this.handleEvent() };
        if (this.method == "get") {
            this.xmlRequest.send(null);
        }
        if (this.method == "post") {
            //test if postText exists and is of type string
            var reallyPost = true;
            if (!this.postText) {
                reallyPost = false;
                alert("Error in network post request: missing parameter 'postText'!");
            }
            if (typeof (this.postText) != "string") {
                reallyPost = false;
                alert("Error in network post request: parameter 'postText' has to be of type 'string')");
            }
            if (reallyPost) {
                this.xmlRequest.send(this.postText);
            }
        }
    }
    //write an error message if neither method is available
    else {
        alert("your browser/svg viewer neither supports window.getURL nor window.XMLHttpRequest!");
    }
}


getData.prototype.operationComplete = function (data) {
    //check if data has a success property
    if (data.success) {
        //parse content of the XML format to the variable "node"
        if (this.returnFormat == "xml") {
            //convert the text information to an XML node and get the first child
            var node = parseXML(data.content, document);
            //distinguish between a callback function and an object
            if (typeof (this.callBackFunction) == "function") {
                this.callBackFunction(node.firstChild, this.additionalParams);
            }
            if (typeof (this.callBackFunction) == "object") {
                this.callBackFunction.receiveData(node.firstChild, this.additionalParams);
            }
        }
        if (this.returnFormat == "json") {
            if (typeof (this.callBackFunction) == "function") {
                this.callBackFunction(data.content, this.additionalParams);
            }
            if (typeof (this.callBackFunction) == "object") {
                this.callBackFunction.receiveData(data.content, this.additionalParams);
            }
        }
    }
    else {
        alert("something went wrong with dynamic loading of geometry!");
    }
}


getData.prototype.handleEvent = function () {
    if (this.xmlRequest.readyState == 4) {
        if (this.returnFormat == "xml") {
            //we need to import the XML node first
            var importedNode = document.importNode(this.xmlRequest.responseXML.documentElement, true);
            if (typeof (this.callBackFunction) == "function") {
                this.callBackFunction(importedNode, this.additionalParams);
            }
            if (typeof (this.callBackFunction) == "object") {
                this.callBackFunction.receiveData(importedNode, this.additionalParams);
            }
        }
        if (this.returnFormat == "json") {
            if (typeof (this.callBackFunction) == "function") {
                this.callBackFunction(this.xmlRequest.responseText, this.additionalParams);
            }
            if (typeof (this.callBackFunction) == "object") {
                this.callBackFunction.receiveData(this.xmlRequest.responseText, this.additionalParams);
            }
        }
    }
}


function serializeNode(node) {
    if (typeof XMLSerializer != 'undefined') {
        return new XMLSerializer().serializeToString(node);
    }
    else if (typeof node.xml != 'undefined') {
        return node.xml;
    }
    else if (typeof printNode != 'undefined') {
        return printNode(node);
    }
    else if (typeof Packages != 'undefined') {
        try {
            var stringWriter = new java.io.StringWriter();
            Packages.org.apache.batik.dom.util.DOMUtilities.writeNode(node, stringWriter);
            return stringWriter.toString();
        }
        catch (e) {
            alert("Sorry, your SVG viewer does not support the printNode/serialize function.");
            return '';
        }
    }
    else {
        alert("Sorry, your SVG viewer does not support the printNode/serialize function.");
        return '';
    }
}

function startAnimation(id) {
    document.getElementById(id).beginElement();
}
// -----------------------------------

/* textFlow.js */
//Copyright (C) <2007>  <Andreas Neumann>
//main function
function textFlow(myText, textToAppend, maxWidth, x, ddy, justified) {
    //extract and add line breaks for start
    var dashArray = new Array();
    var dashFound = true;
    var indexPos = 0;
    var cumulY = 0;
    while (dashFound == true) {
        var result = myText.indexOf("-", indexPos);
        if (result == -1) {
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
        if (computedTextLength > maxWidth || i == 0) {
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
            tspanEl = document.createElementNS(svgNS, "tspan");
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
            if (i != 0) {
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
        if (i == words.length - 1) {
            if (computedTextLength > maxWidth) {
                var tempText = tspanEl.firstChild.nodeValue;
                tspanEl.firstChild.nodeValue = tempText.slice(0, (tempText.length - words[i].length - 1));
                tspanEl = document.createElementNS(svgNS, "tspan");
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

//this function checks if there should be a dash at the given position, instead of a blank
function checkDashPosition(dashArray, pos) {
    var result = false;
    for (var i = 0; i < dashArray.length; i++) {
        if (dashArray[i] == pos) {
            result = true;
        }
    }
    return result;
}
// -----------------------------------