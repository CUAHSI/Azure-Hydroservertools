
//JavaScript random ID generator

//Source: http://snipplr.com/view/68597/generate-unique-id/

//Algorithm Tests:
//See: http://jsfiddle.net/etjyk42f/29/  - uniqIDLength: 10; 1,000,000 IDs generated - no collisions 
//See: http://jsfiddle.net/etjyk42f/31/  - uniqIDLength: 10; 2,000,000 IDs generated - no collisions
//See: http://jsfiddle.net/etjyk42f/32/  - uniqIDLength: 10; 3,000,000 IDs generated - no collisions
//See: http://jsfiddle.net/etjyk42f/33/  - uniqIDLength: 10; 4,000,000 IDs generated - no collisions
//
//NOTE: jsfiddle fails to run 5,000,000 Id test - need to run such tests in your own browser!!


//Implementation - Dynamic Prototype Pattern...

function RandomId(properties) {
    //Properties format: 
    // { 'iterationCount': <n>,                             //Default: 10, Range: 5-10
    //   'characterSets': ['alpha', 'numeric', 'symbol']    //Any combination, case-sensitive: Default: 'alpha'
    // }

    //Validate/intialize input parameters...
    this.properties = properties || {};

    if (("undefined" === typeof this.properties.iterationCount) ||
        (null === this.properties.iterationCount) ||
        (isNaN(this.properties.iterationCount))) {
        this.properties.iterationCount = 10;
    }

    if (("undefined" === typeof this.properties.characterSets) ||
        (null === this.properties.characterSets) ||
        (! Array.isArray(this.properties.characterSets)) ) {
        this.properties.characterSets = ['alpha'];
    }

    //Range/contents checks...
    if ((5 > this.properties.iterationCount) || (10 < this.properties.iterationCount)) {
        throw new RangeError("iterationCount range: 5-10!!");
    }

    var bFound = false;
    var length = this.properties.characterSets.length;
    var sets = ['alpha', 'numeric', 'symbol'];

    for (var i = 0; i < length; ++i ) {
        var set = this.properties.characterSets[i];

        if (-1 !== sets.indexOf(set)) {
            bFound = true;
            break;
        }
    }

    if (!bFound) {
        throw new Error('characterSets must include at least one of the following: ' + sets.toString());
    }

    //Methods...
    var alphaChars = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
    var numericChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    var symbolChars = ['!', '@', '£', '$', '%', '^', '*', '(', ')', '_', '-', '~'];

    if ("function" !== typeof this.generateId) {
        RandomId.prototype.generateId = function () {

            var iterationCount = this.properties.iterationCount;
            var characterSets = this.properties.characterSets;

            var useAlpha = (-1 !== characterSets.indexOf('alpha')) ? true : false;
            var useNumeric = (-1 !== characterSets.indexOf('numeric')) ? true : false;
            var useSymbol = (-1 !== characterSets.indexOf('symbol')) ? true : false;

            var id = ''; 
            for (var i = 0; i < iterationCount; ++i) {

                if (useAlpha) {
                    id += alphaChars[Math.round(Math.random() * 25).toString()];
                }

                if (useNumeric) {
                    id += numericChars[Math.round(Math.random() * 9).toString()]
                }

                if (useSymbol) {
                    Id += symbolChars[Math.round(Math.random() * 11).toString()];
                }
            }

            return (id);
        }
    }
}


/* Test code for original algorithm... 
function generateUniqueID(uniqIDLength) {
    if (!uniqIDLength)
        var uniqIDLength = 10;

    var uniqueID = '', dateStamp = Date().toString().replace(/\s/g, '');
    var alphaChar = '';
    var numericChar = '';
    var symbolChar = '';
    for (var uniqIDCounter = 0; uniqIDCounter < uniqIDLength; uniqIDCounter++) {
        //console.log("Math random: " + Math.round(Math.random() * 25).toString());
        alphaChar = alphaChars[Math.round(Math.random() * 25).toString()];
        numericChar = numericChars[Math.round(Math.random() * 9).toString()]
        //symbolChar = symbolChars[Math.round(Math.random() * 11).toString()];
        console.log("alphaChar: " + alphaChar);
        console.log("numericChar: " + numericChar);
        //console.log("symbolChar: " +  symbolChar);

        uniqueID += alphaChar;
        uniqueID += numericChar;
        //uniqueID += symbolChar;

        //uniqueID += alphaChars[Math.round(Math.random() * 25).toString()];
        //uniqueID += symbolChars[Math.round(Math.random() * 11).toString()];

        //uniqueID += symbolChars[Math.round(Math.random() * (symbolChars.length - 1)).toString()];
        //uniqueID += Math.round(Math.random() * 10);
        //uniqueID += dateStamp.charAt(Math.random() * (dateStamp.length - 1));
    }

    return uniqueID;
}


function hash() {
    //    return Math.floor( Date.now() * Math.random() * Math.random() );
    //return (generateUniqueID(10));    //Generated IDs are 31 characters long
    return (generateUniqueID(5));    //Generated IDs are 15 characters long
}



// Create array with 100 hashes, sorted for visual aid
var test = [];
//for (var i=0; i<1000000; i++){ test.push( hash() ) }
for (var i = 0; i < 1; i++) { test.push(hash()) }
test.sort(function (a, b) { return a - b });

console.log(test);

// Check for duplicate hash -- should never find one
alert("Starting comparison!!");
for (var i = 0; i < test.length; i++) {
    if (i === 0) continue;
    else if (test[i] === test[i - 1]) {
        console.log(">>>>>>>>>>>>>>>>>>>>>>>> Error: Found duplicate hash <<<<<<<<<<<<<<<<<<<<<<<<<<<");
        //break;
    }
}
alert("Comparison Finished!!");
*/