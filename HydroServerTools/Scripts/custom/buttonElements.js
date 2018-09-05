//
//A JavaScript 'enum' for button element values
//NOTE: Values defined here and in any C# equivalent MUST always match!!

//Implementation - Dynamic Prototype Pattern...

// Usage:   var buttonElements = (new ButtonElements()).getEnum();
//          buttonElements.<button name> - 'enum' value - <n>
//          buttonElements.properties[buttonElements.<button name>].description - 'enum' description - '<button name description>'

function ButtonElements() {

    //Enum values...
    //OR'able 
    this.enumValues = {
        "AddFiles": [1, "Add Files Button"],
        "Cancel":   [2, "Cancel Button"],
        "Upload":   [4, "Upload Button"],
        "Insert":   [8, "Insert Button"],
        "Continue": [16, "Continue Button"]
    };

    if ("function" !== typeof this.getEnum) {
        ButtonElements.prototype.getEnum = function () {
            return (freezeEnum(this.enumValues));
        }
    }
}
