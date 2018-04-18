//
//A JavaScript 'enum' for enable/disable action values
//NOTE: Values defined here and in any C# equivalent MUST always match!!

//Implementation - Dynamic Prototype Pattern...

// Usage:   var enableDisableAction = (new EnableDisableAction()).getEnum();
//          enableDisableAction.Enable - 'enum' value - 1
//          enableDisableAction.properties[enableDisable.Enable].description - 'enum' description - 'Enable'

function EnableDisableAction() {
         
    //Enum values...
    this.enumValues = {
        "Enable": [1, "Enable"],
        "Disable": [2, "Disable"]
    };

    if ("function" !== typeof this.getEnum) {
        EnableDisableAction.prototype.getEnum = function () {
            return (freezeEnum(this.enumValues));
        }
    }
}
