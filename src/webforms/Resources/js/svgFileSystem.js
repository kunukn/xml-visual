// FileSystem.js
var ElementFSHeight = 20;
var FS_DISPLAY_ON = "display:inline;";  //opacity: 1;  display:inline;
var FS_DISPLAY_OFF = "display:none;";  //opacity: 0.1;  display:none;

function fsDebug(evt) {} // dev mode

// used by updateSubGroupFS
function fsPush(str, newItem) {
    if (!isDefined(str)) {
        return newItem;
    }
    return str + ";" + newItem;
}
function fsGetPop(str) {
    if (str.indexOf(";") > 0) {
        var s = str.substring(str.lastIndexOf(";") + 1);
        return s;
    }
    else if (str === "") {
        return "";
    }
    else {
        return str;
    }
}
function fsUpdatePop(str) {
    if (str.indexOf(";") > 0) {
        var s = str.substring(0, str.lastIndexOf(";"));
        return s;
    }
    else {
        return "";
    }
}

function updateSubGroupFS(node, y, isExpand) {
    if (isDefined(node) && node.nodeName === "g") {
        var nid = node.getAttributeNS(null, "id");
        if (isDefined(nid)) {
            var isSub = nid.indexOf("subg") === 0;
            if (isSub) {
                var subgroupmode = node.getAttributeNS(null, _XV_MODE);
            }
            var i = 0;
            for (i = 0; i < node.childNodes.length; i++) {
                var c = node.childNodes[i];
                if (c.nodeName === "rect" || c.nodeName === "use" || c.nodeName === "text") {

                    var attrVirtualY = c.getAttributeNS(null, _XV_VIRTUALY);
                    if (!isDefined(attrVirtualY)) {
                        attrVirtualY = "";
                    }
                    if (isExpand) {
                        c.setAttributeNS(null, _XV_VIRTUALY, fsUpdatePop(attrVirtualY));
                    }
                    else {
                        var ystr = ""; ystr = ystr + y;
                        c.setAttributeNS(null, _XV_VIRTUALY, fsPush(attrVirtualY, ystr));
                    }
                }
            }
            var k = 0;
            for (k = 0; k < node.childNodes.length; k++) {
                var cc = node.childNodes[k];
                updateSubGroupFS(cc, y, isExpand);
            }
        }
    }
}

function updateVirtualy(virtualy, yoffset) {
    if (!isDefined(virtualy)) {
        return virtualy;
    }

    var result = "";
    if (virtualy.indexOf(";") > 0) {
        var str = virtualy;
        while (str.indexOf(";") > 0) {
            var s = str.substring(str.lastIndexOf(";") + 1); // one
            var i = parseInt(s, 10);
            result = result + (i + yoffset) + ";";

            str = str.substring(0, str.lastIndexOf(";")); //update                            
        }
        result = result + (parseInt(str, 10) + yoffset);
    }
    else {
        result = "";
        result = result + (parseInt(virtualy, 10) + yoffset);
    }

    return result;
}

// file system view
function recursiveFS(node, gid, y, movement, isExpand) {
    if (isDefined(node) && node.nodeName === "g") {

        var nid = node.getAttributeNS(null, "id");
        if (isDefined(nid)) {
            var index = nid.indexOf(gid);
            var indexSub = nid.indexOf(gid + "_");           
            if (indexSub !== 0 && nid !== gid) { // not inside click and the children of click event
                var i = 0;
                for (i = 0; i < node.childNodes.length; i++) {
                    var c = node.childNodes[i];

                    if (c.nodeName === "rect" || c.nodeName === "use" || c.nodeName === "text") {
                        var cy = parseInt(c.getAttributeNS(null, "y"), 10);
                        var virtualy = cy;
                        var vy = c.getAttributeNS(null, _XV_VIRTUALY);
                        if (isDefined(vy)) {
                            virtualy = parseInt(fsGetPop(vy), 10);
                        }
                        // 15 is because text and rect has diff y, use margin
                        if (virtualy > y + 15) {
                            var ystep;
                            if (isExpand) { ystep = movement; }
                            else { ystep = -movement; }

                            var cyystep = ""; cyystep = cyystep + (cy + ystep);
                            c.setAttributeNS(null, "y", cyystep);
                            //animateYpos(c, (cy + ystep)); //experiment with anim close, open

                            if (isDefined(vy)) {
                                var updatedvy = updateVirtualy(vy, ystep);
                                c.setAttributeNS(null, _XV_VIRTUALY, updatedvy);
                            }
                        }
                    }
                }
            }

            var k = 0;
            for (k = 0; k < node.childNodes.length; k++) {
                var cc = node.childNodes[k];
                recursiveFS(cc, gid, y, movement, isExpand);
            }
        }
    }
}

function fs(evt) {
    var element = evt.target;
    var c = parseInt(evt.target.getAttributeNS(null, _XV_C), 10); //children count
    var xv_virtualy = element.getAttributeNS(null, _XV_VIRTUALY);
    var isEnabled = (!isDefined(xv_virtualy));

    if (c > 0 && isEnabled) {
        var id = evt.target.getAttributeNS(null, "id");
        var y = parseInt(evt.target.getAttributeNS(null, "y"), 10);
        var gid = "g" + id;
        var subgid = "subg" + id;
        var group = document.getElementById(gid);
        var subgroup = document.getElementById(subgid);
        var subgroupmode = subgroup.getAttributeNS(null, _XV_MODE);
        var isExpand;

        if (subgroupmode === "Collapsed") {
            isExpand = true;
            subgroup.setAttributeNS(null, "style", FS_DISPLAY_ON);
            subgroup.setAttributeNS(null, _XV_MODE, "Expanded");
            group.setAttributeNS(null, _XV_MODE, "Expanded");
            document.getElementById("t" + id).setAttributeNS(XLINKNS, "xlink:href", "#symbolExpanded");
        }
        else {
            isExpand = false;
            subgroup.setAttributeNS(null, "style", FS_DISPLAY_OFF);
            subgroup.setAttributeNS(null, _XV_MODE, "Collapsed");
            group.setAttributeNS(null, _XV_MODE, "Collapsed");
            document.getElementById("t" + id).setAttributeNS(XLINKNS, "xlink:href", "#symbolCollapsed");
        }

        updateSubGroupFS(subgroup, y, isExpand); // update virtual y

        // update child count info
        if (id.indexOf("_") > 0) {
            var str = id;
            while (str.indexOf("_") > 0) {
                str = str.substring(0, str.lastIndexOf("_"));
                var ancestorRect = document.getElementById(str);
                var ac = parseInt(ancestorRect.getAttributeNS(null, _XV_C), 10); //children count
                var acPlusc = ""; acPlusc = acPlusc + (ac + c);
                var acMinusc = ""; acMinusc = acMinusc + (ac - c);
                if (isExpand) { ancestorRect.setAttributeNS(null, _XV_C, acPlusc); }
                else { ancestorRect.setAttributeNS(null, _XV_C, acMinusc); }
            }
        }

        var span = 10; // air between nodes
        var movement = c * (ElementFSHeight + span);

        var structure = document.getElementById("structure");
        var i = 0;
        for (i = 0; i < structure.childNodes.length; i++) {
            if (structure.childNodes[i].nodeName === "g") {
                recursiveFS(structure.childNodes[i], gid, y, movement, isExpand);
            }
        }
    }
}
