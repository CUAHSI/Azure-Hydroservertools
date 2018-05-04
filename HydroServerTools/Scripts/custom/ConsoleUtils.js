//
// A simple file defining some console-related utilities...
//

//Enable/Disable console functions per the input boolean
// true: Enable console functions, false: Disable console functions...
//Sources: https://www.codebyamir.com/blog/suppressing-console-log-messages-in-production
//         https://stackoverflow.com/questions/1215392/how-to-quickly-and-conveniently-disable-all-console-log-statements-in-my-code
function EnableDisableConsole(bEnableDisable) {
    //Validate/initialize input parameters...
    if ('undefined' !== typeof bEnableDisable && null !== bEnableDisable) {
        //Input parameters valid - evaluate...
        var originalProperties = console.originalProperties;

        if (bEnableDisable) {
            //Enable requested - check if console disabled...
            if ('undefined' !== typeof originalProperties && null !== originalProperties) {
                //Console disabled - retrieve original properties
                for (var eProp in originalProperties) {
                    console[eProp] = originalProperties[eProp];
                }

                //Remove original properties...
                originalProperties = null;
                delete console.originalProperties;
            }
        }
        else {
            //Disable requested - check if console enabled...
            if ('undefined' === typeof originalProperties || null === originalProperties) {
                //Console enabled - retain/replace original properties...
                originalProperties = {};
                for (var dProp in console) {
                    originalProperties[dProp] = console[dProp];
                    console[dProp] = function () { };
                }

                console.originalProperties = originalProperties;
            }
        }
    }
}