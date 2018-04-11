//
//A JavaScript 'enum' for visibility state values
//NOTE: Values defined here and in any C# equivalent MUST always match!!

//Implementation - Dynamic Prototype Pattern...

// Usage:   var visibilityState = (new VisibilityState()).getEnum();
//          visibilityState.Shown - 'enum' value - 1
//          visibilityState.properties[visibilityState.Shown].description - 'enum' description - 'Shown'

function VisibilityState() {

    //Enum values...
    this.enumValues = {
        "Shown": [1, "Shown"],
        "Hidden": [2, "Hidden"]
    };

    if ("function" !== typeof this.getEnum) {
        VisibilityState.prototype.getEnum = function () {
            return (freezeEnum(this.enumValues));
        }
    }
}
