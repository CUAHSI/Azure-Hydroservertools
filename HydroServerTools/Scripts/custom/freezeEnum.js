
//A utility function to produce a 'frozen' JavaScript 'enum' per the input values

//Expected input format:  { "<EnumName>": [<Enum Numeric Value>,"<Enum Description>"], ... }
//                        NO spaces in <EnumName> !!
//Generated output format: { "<EnumName>": <Enum Numeric Value>, ... ,
//                            "properties": {"<Enum Numeric Value>": {"value": <Enum Numeric Value>,
//                                                                    "description": "<Enum Description>"}, ... }}
//Source: https://stijndewitt.wordpress.com/2014/01/26/enums-in-javascript/ 
//            Response from g2-754970d3251f37967021c7bc09fed8ff September 5, 2014 at 19:53

// For testing please see: https://jsfiddle.net/p8zd6q82/1/

function freezeEnum(enumValues) {
    var properties = {};

    //console.log(JSON.stringify(enumValues));

    for (var index in enumValues) {
        var values = enumValues[index];

        //NOTE: Two assignments here!!  
        //      The 'inner' assignment **REPLACES** the array at enumValues[index] with the value 
        //        of the first element in the array
        //      The 'outer' assignment appends the value/description object to the end of the 
        //        properties object...
        var propIndex = (enumValues[index] = values[0]);
        properties[propIndex] = { "value": values[0], "description": values[1] };

        if ('function' === typeof Object.freeze) {
            Object.freeze(properties[propIndex]);
        }
    }

    enumValues.properties = properties;

    return Object.freeze ? Object.freeze(enumValues) : enumValues;
}
