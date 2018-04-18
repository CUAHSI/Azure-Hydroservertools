//
//A JavaScript 'enum' for status values
//NOTE: Values defined here and in any C# equivalent MUST always match!!

//Implementation - Dynamic Prototype Pattern...

// Usage:   var statusValues = (new StatusValues()).getEnum();
//          statusValues.notStarted - 'enum' value - 1
//          statusValue.properties[statusValues.notStarted].description - 'enum' description - 'notStarted'

function StatusValues() {

    //Enum values...
    this.enumValues = {
        "notStarted": [1, "notStarted"],
        "inProgress": [2, "inProgress"],
        "complete": [3, "complete"],
        "error": [4, "error"]
    };

    if ("function" !== typeof this.getEnum) {
        StatusValues.prototype.getEnum = function () {
            return (freezeEnum(this.enumValues));
        }
    }
}
