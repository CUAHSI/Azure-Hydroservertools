/*
 * This combined file was created by the DataTables downloader builder:
 *   https://datatables.net/download
 *
 * To rebuild or modify this file with the latest versions of the included
 * software please visit:
 *   https://datatables.net/download/#dt/dt-1.10.16/e-1.7.0/b-1.5.1/sl-1.2.4
 *
 * Included libraries:
 *   DataTables 1.10.16, Editor 1.7.0, Buttons 1.5.1, Select 1.2.4
 */

/*! DataTables 1.10.16
 * ©2008-2017 SpryMedia Ltd - datatables.net/license
 */

/**
 * @summary     DataTables
 * @description Paginate, search and order HTML tables
 * @version     1.10.16
 * @file        jquery.dataTables.js
 * @author      SpryMedia Ltd
 * @contact     www.datatables.net
 * @copyright   Copyright 2008-2017 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net
 */

/*jslint evil: true, undef: true, browser: true */
/*globals $,require,jQuery,define,_selector_run,_selector_opts,_selector_first,_selector_row_indexes,_ext,_Api,_api_register,_api_registerPlural,_re_new_lines,_re_html,_re_formatted_numeric,_re_escape_regex,_empty,_intVal,_numToDecimal,_isNumber,_isHtml,_htmlNumeric,_pluck,_pluck_order,_range,_stripHtml,_unique,_fnBuildAjax,_fnAjaxUpdate,_fnAjaxParameters,_fnAjaxUpdateDraw,_fnAjaxDataSrc,_fnAddColumn,_fnColumnOptions,_fnAdjustColumnSizing,_fnVisibleToColumnIndex,_fnColumnIndexToVisible,_fnVisbleColumns,_fnGetColumns,_fnColumnTypes,_fnApplyColumnDefs,_fnHungarianMap,_fnCamelToHungarian,_fnLanguageCompat,_fnBrowserDetect,_fnAddData,_fnAddTr,_fnNodeToDataIndex,_fnNodeToColumnIndex,_fnGetCellData,_fnSetCellData,_fnSplitObjNotation,_fnGetObjectDataFn,_fnSetObjectDataFn,_fnGetDataMaster,_fnClearTable,_fnDeleteIndex,_fnInvalidate,_fnGetRowElements,_fnCreateTr,_fnBuildHead,_fnDrawHead,_fnDraw,_fnReDraw,_fnAddOptionsHtml,_fnDetectHeader,_fnGetUniqueThs,_fnFeatureHtmlFilter,_fnFilterComplete,_fnFilterCustom,_fnFilterColumn,_fnFilter,_fnFilterCreateSearch,_fnEscapeRegex,_fnFilterData,_fnFeatureHtmlInfo,_fnUpdateInfo,_fnInfoMacros,_fnInitialise,_fnInitComplete,_fnLengthChange,_fnFeatureHtmlLength,_fnFeatureHtmlPaginate,_fnPageChange,_fnFeatureHtmlProcessing,_fnProcessingDisplay,_fnFeatureHtmlTable,_fnScrollDraw,_fnApplyToChildren,_fnCalculateColumnWidths,_fnThrottle,_fnConvertToWidth,_fnGetWidestNode,_fnGetMaxLenString,_fnStringToCss,_fnSortFlatten,_fnSort,_fnSortAria,_fnSortListener,_fnSortAttachListener,_fnSortingClasses,_fnSortData,_fnSaveState,_fnLoadState,_fnSettingsFromNode,_fnLog,_fnMap,_fnBindAction,_fnCallbackReg,_fnCallbackFire,_fnLengthOverflow,_fnRenderer,_fnDataSource,_fnRowAttributes*/

(function( factory ) {
	"use strict";

	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				// CommonJS environments without a window global must pass a
				// root. This will give an error otherwise
				root = window;
			}

			if ( ! $ ) {
				$ = typeof window !== 'undefined' ? // jQuery's factory checks for a global window
					require('jquery') :
					require('jquery')( root );
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}
(function( $, window, document, undefined ) {
	"use strict";

	/**
	 * DataTables is a plug-in for the jQuery Javascript library. It is a highly
	 * flexible tool, based upon the foundations of progressive enhancement,
	 * which will add advanced interaction controls to any HTML table. For a
	 * full list of features please refer to
	 * [DataTables.net](href="http://datatables.net).
	 *
	 * Note that the `DataTable` object is not a global variable but is aliased
	 * to `jQuery.fn.DataTable` and `jQuery.fn.dataTable` through which it may
	 * be  accessed.
	 *
	 *  @class
	 *  @param {object} [init={}] Configuration object for DataTables. Options
	 *    are defined by {@link DataTable.defaults}
	 *  @requires jQuery 1.7+
	 *
	 *  @example
	 *    // Basic initialisation
	 *    $(document).ready( function {
	 *      $('#example').dataTable();
	 *    } );
	 *
	 *  @example
	 *    // Initialisation with configuration options - in this case, disable
	 *    // pagination and sorting.
	 *    $(document).ready( function {
	 *      $('#example').dataTable( {
	 *        "paginate": false,
	 *        "sort": false
	 *      } );
	 *    } );
	 */
	var DataTable = function ( options )
	{
		/**
		 * Perform a jQuery selector action on the table's TR elements (from the tbody) and
		 * return the resulting jQuery object.
		 *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
		 *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
		 *  @param {string} [oOpts.filter=none] Select TR elements that meet the current filter
		 *    criterion ("applied") or all TR elements (i.e. no filter).
		 *  @param {string} [oOpts.order=current] Order of the TR elements in the processed array.
		 *    Can be either 'current', whereby the current sorting of the table is used, or
		 *    'original' whereby the original order the data was read into the table is used.
		 *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
		 *    ("current") or not ("all"). If 'current' is given, then order is assumed to be
		 *    'current' and filter is 'applied', regardless of what they might be given as.
		 *  @returns {object} jQuery object, filtered by the given selector.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Highlight every second row
		 *      oTable.$('tr:odd').css('backgroundColor', 'blue');
		 *    } );
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Filter to rows with 'Webkit' in them, add a background colour and then
		 *      // remove the filter, thus highlighting the 'Webkit' rows only.
		 *      oTable.fnFilter('Webkit');
		 *      oTable.$('tr', {"search": "applied"}).css('backgroundColor', 'blue');
		 *      oTable.fnFilter('');
		 *    } );
		 */
		this.$ = function ( sSelector, oOpts )
		{
			return this.api(true).$( sSelector, oOpts );
		};
		
		
		/**
		 * Almost identical to $ in operation, but in this case returns the data for the matched
		 * rows - as such, the jQuery selector used should match TR row nodes or TD/TH cell nodes
		 * rather than any descendants, so the data can be obtained for the row/cell. If matching
		 * rows are found, the data returned is the original data array/object that was used to
		 * create the row (or a generated array if from a DOM source).
		 *
		 * This method is often useful in-combination with $ where both functions are given the
		 * same parameters and the array indexes will match identically.
		 *  @param {string|node|jQuery} sSelector jQuery selector or node collection to act on
		 *  @param {object} [oOpts] Optional parameters for modifying the rows to be included
		 *  @param {string} [oOpts.filter=none] Select elements that meet the current filter
		 *    criterion ("applied") or all elements (i.e. no filter).
		 *  @param {string} [oOpts.order=current] Order of the data in the processed array.
		 *    Can be either 'current', whereby the current sorting of the table is used, or
		 *    'original' whereby the original order the data was read into the table is used.
		 *  @param {string} [oOpts.page=all] Limit the selection to the currently displayed page
		 *    ("current") or not ("all"). If 'current' is given, then order is assumed to be
		 *    'current' and filter is 'applied', regardless of what they might be given as.
		 *  @returns {array} Data for the matched elements. If any elements, as a result of the
		 *    selector, were not TR, TD or TH elements in the DataTable, they will have a null
		 *    entry in the array.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Get the data from the first row in the table
		 *      var data = oTable._('tr:first');
		 *
		 *      // Do something useful with the data
		 *      alert( "First cell is: "+data[0] );
		 *    } );
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Filter to 'Webkit' and get all data for
		 *      oTable.fnFilter('Webkit');
		 *      var data = oTable._('tr', {"search": "applied"});
		 *
		 *      // Do something with the data
		 *      alert( data.length+" rows matched the search" );
		 *    } );
		 */
		this._ = function ( sSelector, oOpts )
		{
			return this.api(true).rows( sSelector, oOpts ).data();
		};
		
		
		/**
		 * Create a DataTables Api instance, with the currently selected tables for
		 * the Api's context.
		 * @param {boolean} [traditional=false] Set the API instance's context to be
		 *   only the table referred to by the `DataTable.ext.iApiIndex` option, as was
		 *   used in the API presented by DataTables 1.9- (i.e. the traditional mode),
		 *   or if all tables captured in the jQuery object should be used.
		 * @return {DataTables.Api}
		 */
		this.api = function ( traditional )
		{
			return traditional ?
				new _Api(
					_fnSettingsFromNode( this[ _ext.iApiIndex ] )
				) :
				new _Api( this );
		};
		
		
		/**
		 * Add a single new row or multiple rows of data to the table. Please note
		 * that this is suitable for client-side processing only - if you are using
		 * server-side processing (i.e. "bServerSide": true), then to add data, you
		 * must add it to the data source, i.e. the server-side, through an Ajax call.
		 *  @param {array|object} data The data to be added to the table. This can be:
		 *    <ul>
		 *      <li>1D array of data - add a single row with the data provided</li>
		 *      <li>2D array of arrays - add multiple rows in a single call</li>
		 *      <li>object - data object when using <i>mData</i></li>
		 *      <li>array of objects - multiple data objects when using <i>mData</i></li>
		 *    </ul>
		 *  @param {bool} [redraw=true] redraw the table or not
		 *  @returns {array} An array of integers, representing the list of indexes in
		 *    <i>aoData</i> ({@link DataTable.models.oSettings}) that have been added to
		 *    the table.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    // Global var for counter
		 *    var giCount = 2;
		 *
		 *    $(document).ready(function() {
		 *      $('#example').dataTable();
		 *    } );
		 *
		 *    function fnClickAddRow() {
		 *      $('#example').dataTable().fnAddData( [
		 *        giCount+".1",
		 *        giCount+".2",
		 *        giCount+".3",
		 *        giCount+".4" ]
		 *      );
		 *
		 *      giCount++;
		 *    }
		 */
		this.fnAddData = function( data, redraw )
		{
			var api = this.api( true );
		
			/* Check if we want to add multiple rows or not */
			var rows = $.isArray(data) && ( $.isArray(data[0]) || $.isPlainObject(data[0]) ) ?
				api.rows.add( data ) :
				api.row.add( data );
		
			if ( redraw === undefined || redraw ) {
				api.draw();
			}
		
			return rows.flatten().toArray();
		};
		
		
		/**
		 * This function will make DataTables recalculate the column sizes, based on the data
		 * contained in the table and the sizes applied to the columns (in the DOM, CSS or
		 * through the sWidth parameter). This can be useful when the width of the table's
		 * parent element changes (for example a window resize).
		 *  @param {boolean} [bRedraw=true] Redraw the table or not, you will typically want to
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable( {
		 *        "sScrollY": "200px",
		 *        "bPaginate": false
		 *      } );
		 *
		 *      $(window).on('resize', function () {
		 *        oTable.fnAdjustColumnSizing();
		 *      } );
		 *    } );
		 */
		this.fnAdjustColumnSizing = function ( bRedraw )
		{
			var api = this.api( true ).columns.adjust();
			var settings = api.settings()[0];
			var scroll = settings.oScroll;
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw( false );
			}
			else if ( scroll.sX !== "" || scroll.sY !== "" ) {
				/* If not redrawing, but scrolling, we want to apply the new column sizes anyway */
				_fnScrollDraw( settings );
			}
		};
		
		
		/**
		 * Quickly and simply clear a table
		 *  @param {bool} [bRedraw=true] redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Immediately 'nuke' the current rows (perhaps waiting for an Ajax callback...)
		 *      oTable.fnClearTable();
		 *    } );
		 */
		this.fnClearTable = function( bRedraw )
		{
			var api = this.api( true ).clear();
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw();
			}
		};
		
		
		/**
		 * The exact opposite of 'opening' a row, this function will close any rows which
		 * are currently 'open'.
		 *  @param {node} nTr the table row to 'close'
		 *  @returns {int} 0 on success, or 1 if failed (can't find the row)
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnClose = function( nTr )
		{
			this.api( true ).row( nTr ).child.hide();
		};
		
		
		/**
		 * Remove a row for the table
		 *  @param {mixed} target The index of the row from aoData to be deleted, or
		 *    the TR element you want to delete
		 *  @param {function|null} [callBack] Callback function
		 *  @param {bool} [redraw=true] Redraw the table or not
		 *  @returns {array} The row that was deleted
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Immediately remove the first row
		 *      oTable.fnDeleteRow( 0 );
		 *    } );
		 */
		this.fnDeleteRow = function( target, callback, redraw )
		{
			var api = this.api( true );
			var rows = api.rows( target );
			var settings = rows.settings()[0];
			var data = settings.aoData[ rows[0][0] ];
		
			rows.remove();
		
			if ( callback ) {
				callback.call( this, settings, data );
			}
		
			if ( redraw === undefined || redraw ) {
				api.draw();
			}
		
			return data;
		};
		
		
		/**
		 * Restore the table to it's original state in the DOM by removing all of DataTables
		 * enhancements, alterations to the DOM structure of the table and event listeners.
		 *  @param {boolean} [remove=false] Completely remove the table from the DOM
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      // This example is fairly pointless in reality, but shows how fnDestroy can be used
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnDestroy();
		 *    } );
		 */
		this.fnDestroy = function ( remove )
		{
			this.api( true ).destroy( remove );
		};
		
		
		/**
		 * Redraw the table
		 *  @param {bool} [complete=true] Re-filter and resort (if enabled) the table before the draw.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Re-draw the table - you wouldn't want to do it here, but it's an example :-)
		 *      oTable.fnDraw();
		 *    } );
		 */
		this.fnDraw = function( complete )
		{
			// Note that this isn't an exact match to the old call to _fnDraw - it takes
			// into account the new data, but can hold position.
			this.api( true ).draw( complete );
		};
		
		
		/**
		 * Filter the input based on data
		 *  @param {string} sInput String to filter the table on
		 *  @param {int|null} [iColumn] Column to limit filtering to
		 *  @param {bool} [bRegex=false] Treat as regular expression or not
		 *  @param {bool} [bSmart=true] Perform smart filtering or not
		 *  @param {bool} [bShowGlobal=true] Show the input global filter in it's input box(es)
		 *  @param {bool} [bCaseInsensitive=true] Do case-insensitive matching (true) or not (false)
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sometime later - filter...
		 *      oTable.fnFilter( 'test string' );
		 *    } );
		 */
		this.fnFilter = function( sInput, iColumn, bRegex, bSmart, bShowGlobal, bCaseInsensitive )
		{
			var api = this.api( true );
		
			if ( iColumn === null || iColumn === undefined ) {
				api.search( sInput, bRegex, bSmart, bCaseInsensitive );
			}
			else {
				api.column( iColumn ).search( sInput, bRegex, bSmart, bCaseInsensitive );
			}
		
			api.draw();
		};
		
		
		/**
		 * Get the data for the whole table, an individual row or an individual cell based on the
		 * provided parameters.
		 *  @param {int|node} [src] A TR row node, TD/TH cell node or an integer. If given as
		 *    a TR node then the data source for the whole row will be returned. If given as a
		 *    TD/TH cell node then iCol will be automatically calculated and the data for the
		 *    cell returned. If given as an integer, then this is treated as the aoData internal
		 *    data index for the row (see fnGetPosition) and the data for that row used.
		 *  @param {int} [col] Optional column index that you want the data of.
		 *  @returns {array|object|string} If mRow is undefined, then the data for all rows is
		 *    returned. If mRow is defined, just data for that row, and is iCol is
		 *    defined, only data for the designated cell is returned.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    // Row data
		 *    $(document).ready(function() {
		 *      oTable = $('#example').dataTable();
		 *
		 *      oTable.$('tr').click( function () {
		 *        var data = oTable.fnGetData( this );
		 *        // ... do something with the array / object of data for the row
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Individual cell data
		 *    $(document).ready(function() {
		 *      oTable = $('#example').dataTable();
		 *
		 *      oTable.$('td').click( function () {
		 *        var sData = oTable.fnGetData( this );
		 *        alert( 'The cell clicked on had the value of '+sData );
		 *      } );
		 *    } );
		 */
		this.fnGetData = function( src, col )
		{
			var api = this.api( true );
		
			if ( src !== undefined ) {
				var type = src.nodeName ? src.nodeName.toLowerCase() : '';
		
				return col !== undefined || type == 'td' || type == 'th' ?
					api.cell( src, col ).data() :
					api.row( src ).data() || null;
			}
		
			return api.data().toArray();
		};
		
		
		/**
		 * Get an array of the TR nodes that are used in the table's body. Note that you will
		 * typically want to use the '$' API method in preference to this as it is more
		 * flexible.
		 *  @param {int} [iRow] Optional row index for the TR element you want
		 *  @returns {array|node} If iRow is undefined, returns an array of all TR elements
		 *    in the table's body, or iRow is defined, just the TR element requested.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Get the nodes from the table
		 *      var nNodes = oTable.fnGetNodes( );
		 *    } );
		 */
		this.fnGetNodes = function( iRow )
		{
			var api = this.api( true );
		
			return iRow !== undefined ?
				api.row( iRow ).node() :
				api.rows().nodes().flatten().toArray();
		};
		
		
		/**
		 * Get the array indexes of a particular cell from it's DOM element
		 * and column index including hidden columns
		 *  @param {node} node this can either be a TR, TD or TH in the table's body
		 *  @returns {int} If nNode is given as a TR, then a single index is returned, or
		 *    if given as a cell, an array of [row index, column index (visible),
		 *    column index (all)] is given.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      $('#example tbody td').click( function () {
		 *        // Get the position of the current data from the node
		 *        var aPos = oTable.fnGetPosition( this );
		 *
		 *        // Get the data array for this row
		 *        var aData = oTable.fnGetData( aPos[0] );
		 *
		 *        // Update the data array and return the value
		 *        aData[ aPos[1] ] = 'clicked';
		 *        this.innerHTML = 'clicked';
		 *      } );
		 *
		 *      // Init DataTables
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnGetPosition = function( node )
		{
			var api = this.api( true );
			var nodeName = node.nodeName.toUpperCase();
		
			if ( nodeName == 'TR' ) {
				return api.row( node ).index();
			}
			else if ( nodeName == 'TD' || nodeName == 'TH' ) {
				var cell = api.cell( node ).index();
		
				return [
					cell.row,
					cell.columnVisible,
					cell.column
				];
			}
			return null;
		};
		
		
		/**
		 * Check to see if a row is 'open' or not.
		 *  @param {node} nTr the table row to check
		 *  @returns {boolean} true if the row is currently open, false otherwise
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnIsOpen = function( nTr )
		{
			return this.api( true ).row( nTr ).child.isShown();
		};
		
		
		/**
		 * This function will place a new row directly after a row which is currently
		 * on display on the page, with the HTML contents that is passed into the
		 * function. This can be used, for example, to ask for confirmation that a
		 * particular record should be deleted.
		 *  @param {node} nTr The table row to 'open'
		 *  @param {string|node|jQuery} mHtml The HTML to put into the row
		 *  @param {string} sClass Class to give the new TD cell
		 *  @returns {node} The row opened. Note that if the table row passed in as the
		 *    first parameter, is not found in the table, this method will silently
		 *    return.
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable;
		 *
		 *      // 'open' an information row when a row is clicked on
		 *      $('#example tbody tr').click( function () {
		 *        if ( oTable.fnIsOpen(this) ) {
		 *          oTable.fnClose( this );
		 *        } else {
		 *          oTable.fnOpen( this, "Temporary row opened", "info_row" );
		 *        }
		 *      } );
		 *
		 *      oTable = $('#example').dataTable();
		 *    } );
		 */
		this.fnOpen = function( nTr, mHtml, sClass )
		{
			return this.api( true )
				.row( nTr )
				.child( mHtml, sClass )
				.show()
				.child()[0];
		};
		
		
		/**
		 * Change the pagination - provides the internal logic for pagination in a simple API
		 * function. With this function you can have a DataTables table go to the next,
		 * previous, first or last pages.
		 *  @param {string|int} mAction Paging action to take: "first", "previous", "next" or "last"
		 *    or page number to jump to (integer), note that page 0 is the first page.
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnPageChange( 'next' );
		 *    } );
		 */
		this.fnPageChange = function ( mAction, bRedraw )
		{
			var api = this.api( true ).page( mAction );
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw(false);
			}
		};
		
		
		/**
		 * Show a particular column
		 *  @param {int} iCol The column whose display should be changed
		 *  @param {bool} bShow Show (true) or hide (false) the column
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Hide the second column after initialisation
		 *      oTable.fnSetColumnVis( 1, false );
		 *    } );
		 */
		this.fnSetColumnVis = function ( iCol, bShow, bRedraw )
		{
			var api = this.api( true ).column( iCol ).visible( bShow );
		
			if ( bRedraw === undefined || bRedraw ) {
				api.columns.adjust().draw();
			}
		};
		
		
		/**
		 * Get the settings for a particular table for external manipulation
		 *  @returns {object} DataTables settings object. See
		 *    {@link DataTable.models.oSettings}
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      var oSettings = oTable.fnSettings();
		 *
		 *      // Show an example parameter from the settings
		 *      alert( oSettings._iDisplayStart );
		 *    } );
		 */
		this.fnSettings = function()
		{
			return _fnSettingsFromNode( this[_ext.iApiIndex] );
		};
		
		
		/**
		 * Sort the table by a particular column
		 *  @param {int} iCol the data index to sort on. Note that this will not match the
		 *    'display index' if you have hidden data entries
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sort immediately with columns 0 and 1
		 *      oTable.fnSort( [ [0,'asc'], [1,'asc'] ] );
		 *    } );
		 */
		this.fnSort = function( aaSort )
		{
			this.api( true ).order( aaSort ).draw();
		};
		
		
		/**
		 * Attach a sort listener to an element for a given column
		 *  @param {node} nNode the element to attach the sort listener to
		 *  @param {int} iColumn the column that a click on this node will sort on
		 *  @param {function} [fnCallback] callback function when sort is run
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *
		 *      // Sort on column 1, when 'sorter' is clicked on
		 *      oTable.fnSortListener( document.getElementById('sorter'), 1 );
		 *    } );
		 */
		this.fnSortListener = function( nNode, iColumn, fnCallback )
		{
			this.api( true ).order.listener( nNode, iColumn, fnCallback );
		};
		
		
		/**
		 * Update a table cell or row - this method will accept either a single value to
		 * update the cell with, an array of values with one element for each column or
		 * an object in the same format as the original data source. The function is
		 * self-referencing in order to make the multi column updates easier.
		 *  @param {object|array|string} mData Data to update the cell/row with
		 *  @param {node|int} mRow TR element you want to update or the aoData index
		 *  @param {int} [iColumn] The column to update, give as null or undefined to
		 *    update a whole row.
		 *  @param {bool} [bRedraw=true] Redraw the table or not
		 *  @param {bool} [bAction=true] Perform pre-draw actions or not
		 *  @returns {int} 0 on success, 1 on error
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      oTable.fnUpdate( 'Example update', 0, 0 ); // Single cell
		 *      oTable.fnUpdate( ['a', 'b', 'c', 'd', 'e'], $('tbody tr')[0] ); // Row
		 *    } );
		 */
		this.fnUpdate = function( mData, mRow, iColumn, bRedraw, bAction )
		{
			var api = this.api( true );
		
			if ( iColumn === undefined || iColumn === null ) {
				api.row( mRow ).data( mData );
			}
			else {
				api.cell( mRow, iColumn ).data( mData );
			}
		
			if ( bAction === undefined || bAction ) {
				api.columns.adjust();
			}
		
			if ( bRedraw === undefined || bRedraw ) {
				api.draw();
			}
			return 0;
		};
		
		
		/**
		 * Provide a common method for plug-ins to check the version of DataTables being used, in order
		 * to ensure compatibility.
		 *  @param {string} sVersion Version string to check for, in the format "X.Y.Z". Note that the
		 *    formats "X" and "X.Y" are also acceptable.
		 *  @returns {boolean} true if this version of DataTables is greater or equal to the required
		 *    version, or false if this version of DataTales is not suitable
		 *  @method
		 *  @dtopt API
		 *  @deprecated Since v1.10
		 *
		 *  @example
		 *    $(document).ready(function() {
		 *      var oTable = $('#example').dataTable();
		 *      alert( oTable.fnVersionCheck( '1.9.0' ) );
		 *    } );
		 */
		this.fnVersionCheck = _ext.fnVersionCheck;
		

		var _that = this;
		var emptyInit = options === undefined;
		var len = this.length;

		if ( emptyInit ) {
			options = {};
		}

		this.oApi = this.internal = _ext.internal;

		// Extend with old style plug-in API methods
		for ( var fn in DataTable.ext.internal ) {
			if ( fn ) {
				this[fn] = _fnExternApiFunc(fn);
			}
		}

		this.each(function() {
			// For each initialisation we want to give it a clean initialisation
			// object that can be bashed around
			var o = {};
			var oInit = len > 1 ? // optimisation for single table case
				_fnExtend( o, options, true ) :
				options;

			/*global oInit,_that,emptyInit*/
			var i=0, iLen, j, jLen, k, kLen;
			var sId = this.getAttribute( 'id' );
			var bInitHandedOff = false;
			var defaults = DataTable.defaults;
			var $this = $(this);
			
			
			/* Sanity check */
			if ( this.nodeName.toLowerCase() != 'table' )
			{
				_fnLog( null, 0, 'Non-table node initialisation ('+this.nodeName+')', 2 );
				return;
			}
			
			/* Backwards compatibility for the defaults */
			_fnCompatOpts( defaults );
			_fnCompatCols( defaults.column );
			
			/* Convert the camel-case defaults to Hungarian */
			_fnCamelToHungarian( defaults, defaults, true );
			_fnCamelToHungarian( defaults.column, defaults.column, true );
			
			/* Setting up the initialisation object */
			_fnCamelToHungarian( defaults, $.extend( oInit, $this.data() ) );
			
			
			
			/* Check to see if we are re-initialising a table */
			var allSettings = DataTable.settings;
			for ( i=0, iLen=allSettings.length ; i<iLen ; i++ )
			{
				var s = allSettings[i];
			
				/* Base check on table node */
				if ( s.nTable == this || s.nTHead.parentNode == this || (s.nTFoot && s.nTFoot.parentNode == this) )
				{
					var bRetrieve = oInit.bRetrieve !== undefined ? oInit.bRetrieve : defaults.bRetrieve;
					var bDestroy = oInit.bDestroy !== undefined ? oInit.bDestroy : defaults.bDestroy;
			
					if ( emptyInit || bRetrieve )
					{
						return s.oInstance;
					}
					else if ( bDestroy )
					{
						s.oInstance.fnDestroy();
						break;
					}
					else
					{
						_fnLog( s, 0, 'Cannot reinitialise DataTable', 3 );
						return;
					}
				}
			
				/* If the element we are initialising has the same ID as a table which was previously
				 * initialised, but the table nodes don't match (from before) then we destroy the old
				 * instance by simply deleting it. This is under the assumption that the table has been
				 * destroyed by other methods. Anyone using non-id selectors will need to do this manually
				 */
				if ( s.sTableId == this.id )
				{
					allSettings.splice( i, 1 );
					break;
				}
			}
			
			/* Ensure the table has an ID - required for accessibility */
			if ( sId === null || sId === "" )
			{
				sId = "DataTables_Table_"+(DataTable.ext._unique++);
				this.id = sId;
			}
			
			/* Create the settings object for this table and set some of the default parameters */
			var oSettings = $.extend( true, {}, DataTable.models.oSettings, {
				"sDestroyWidth": $this[0].style.width,
				"sInstance":     sId,
				"sTableId":      sId
			} );
			oSettings.nTable = this;
			oSettings.oApi   = _that.internal;
			oSettings.oInit  = oInit;
			
			allSettings.push( oSettings );
			
			// Need to add the instance after the instance after the settings object has been added
			// to the settings array, so we can self reference the table instance if more than one
			oSettings.oInstance = (_that.length===1) ? _that : $this.dataTable();
			
			// Backwards compatibility, before we apply all the defaults
			_fnCompatOpts( oInit );
			
			if ( oInit.oLanguage )
			{
				_fnLanguageCompat( oInit.oLanguage );
			}
			
			// If the length menu is given, but the init display length is not, use the length menu
			if ( oInit.aLengthMenu && ! oInit.iDisplayLength )
			{
				oInit.iDisplayLength = $.isArray( oInit.aLengthMenu[0] ) ?
					oInit.aLengthMenu[0][0] : oInit.aLengthMenu[0];
			}
			
			// Apply the defaults and init options to make a single init object will all
			// options defined from defaults and instance options.
			oInit = _fnExtend( $.extend( true, {}, defaults ), oInit );
			
			
			// Map the initialisation options onto the settings object
			_fnMap( oSettings.oFeatures, oInit, [
				"bPaginate",
				"bLengthChange",
				"bFilter",
				"bSort",
				"bSortMulti",
				"bInfo",
				"bProcessing",
				"bAutoWidth",
				"bSortClasses",
				"bServerSide",
				"bDeferRender"
			] );
			_fnMap( oSettings, oInit, [
				"asStripeClasses",
				"ajax",
				"fnServerData",
				"fnFormatNumber",
				"sServerMethod",
				"aaSorting",
				"aaSortingFixed",
				"aLengthMenu",
				"sPaginationType",
				"sAjaxSource",
				"sAjaxDataProp",
				"iStateDuration",
				"sDom",
				"bSortCellsTop",
				"iTabIndex",
				"fnStateLoadCallback",
				"fnStateSaveCallback",
				"renderer",
				"searchDelay",
				"rowId",
				[ "iCookieDuration", "iStateDuration" ], // backwards compat
				[ "oSearch", "oPreviousSearch" ],
				[ "aoSearchCols", "aoPreSearchCols" ],
				[ "iDisplayLength", "_iDisplayLength" ]
			] );
			_fnMap( oSettings.oScroll, oInit, [
				[ "sScrollX", "sX" ],
				[ "sScrollXInner", "sXInner" ],
				[ "sScrollY", "sY" ],
				[ "bScrollCollapse", "bCollapse" ]
			] );
			_fnMap( oSettings.oLanguage, oInit, "fnInfoCallback" );
			
			/* Callback functions which are array driven */
			_fnCallbackReg( oSettings, 'aoDrawCallback',       oInit.fnDrawCallback,      'user' );
			_fnCallbackReg( oSettings, 'aoServerParams',       oInit.fnServerParams,      'user' );
			_fnCallbackReg( oSettings, 'aoStateSaveParams',    oInit.fnStateSaveParams,   'user' );
			_fnCallbackReg( oSettings, 'aoStateLoadParams',    oInit.fnStateLoadParams,   'user' );
			_fnCallbackReg( oSettings, 'aoStateLoaded',        oInit.fnStateLoaded,       'user' );
			_fnCallbackReg( oSettings, 'aoRowCallback',        oInit.fnRowCallback,       'user' );
			_fnCallbackReg( oSettings, 'aoRowCreatedCallback', oInit.fnCreatedRow,        'user' );
			_fnCallbackReg( oSettings, 'aoHeaderCallback',     oInit.fnHeaderCallback,    'user' );
			_fnCallbackReg( oSettings, 'aoFooterCallback',     oInit.fnFooterCallback,    'user' );
			_fnCallbackReg( oSettings, 'aoInitComplete',       oInit.fnInitComplete,      'user' );
			_fnCallbackReg( oSettings, 'aoPreDrawCallback',    oInit.fnPreDrawCallback,   'user' );
			
			oSettings.rowIdFn = _fnGetObjectDataFn( oInit.rowId );
			
			/* Browser support detection */
			_fnBrowserDetect( oSettings );
			
			var oClasses = oSettings.oClasses;
			
			$.extend( oClasses, DataTable.ext.classes, oInit.oClasses );
			$this.addClass( oClasses.sTable );
			
			
			if ( oSettings.iInitDisplayStart === undefined )
			{
				/* Display start point, taking into account the save saving */
				oSettings.iInitDisplayStart = oInit.iDisplayStart;
				oSettings._iDisplayStart = oInit.iDisplayStart;
			}
			
			if ( oInit.iDeferLoading !== null )
			{
				oSettings.bDeferLoading = true;
				var tmp = $.isArray( oInit.iDeferLoading );
				oSettings._iRecordsDisplay = tmp ? oInit.iDeferLoading[0] : oInit.iDeferLoading;
				oSettings._iRecordsTotal = tmp ? oInit.iDeferLoading[1] : oInit.iDeferLoading;
			}
			
			/* Language definitions */
			var oLanguage = oSettings.oLanguage;
			$.extend( true, oLanguage, oInit.oLanguage );
			
			if ( oLanguage.sUrl )
			{
				/* Get the language definitions from a file - because this Ajax call makes the language
				 * get async to the remainder of this function we use bInitHandedOff to indicate that
				 * _fnInitialise will be fired by the returned Ajax handler, rather than the constructor
				 */
				$.ajax( {
					dataType: 'json',
					url: oLanguage.sUrl,
					success: function ( json ) {
						_fnLanguageCompat( json );
						_fnCamelToHungarian( defaults.oLanguage, json );
						$.extend( true, oLanguage, json );
						_fnInitialise( oSettings );
					},
					error: function () {
						// Error occurred loading language file, continue on as best we can
						_fnInitialise( oSettings );
					}
				} );
				bInitHandedOff = true;
			}
			
			/*
			 * Stripes
			 */
			if ( oInit.asStripeClasses === null )
			{
				oSettings.asStripeClasses =[
					oClasses.sStripeOdd,
					oClasses.sStripeEven
				];
			}
			
			/* Remove row stripe classes if they are already on the table row */
			var stripeClasses = oSettings.asStripeClasses;
			var rowOne = $this.children('tbody').find('tr').eq(0);
			if ( $.inArray( true, $.map( stripeClasses, function(el, i) {
				return rowOne.hasClass(el);
			} ) ) !== -1 ) {
				$('tbody tr', this).removeClass( stripeClasses.join(' ') );
				oSettings.asDestroyStripes = stripeClasses.slice();
			}
			
			/*
			 * Columns
			 * See if we should load columns automatically or use defined ones
			 */
			var anThs = [];
			var aoColumnsInit;
			var nThead = this.getElementsByTagName('thead');
			if ( nThead.length !== 0 )
			{
				_fnDetectHeader( oSettings.aoHeader, nThead[0] );
				anThs = _fnGetUniqueThs( oSettings );
			}
			
			/* If not given a column array, generate one with nulls */
			if ( oInit.aoColumns === null )
			{
				aoColumnsInit = [];
				for ( i=0, iLen=anThs.length ; i<iLen ; i++ )
				{
					aoColumnsInit.push( null );
				}
			}
			else
			{
				aoColumnsInit = oInit.aoColumns;
			}
			
			/* Add the columns */
			for ( i=0, iLen=aoColumnsInit.length ; i<iLen ; i++ )
			{
				_fnAddColumn( oSettings, anThs ? anThs[i] : null );
			}
			
			/* Apply the column definitions */
			_fnApplyColumnDefs( oSettings, oInit.aoColumnDefs, aoColumnsInit, function (iCol, oDef) {
				_fnColumnOptions( oSettings, iCol, oDef );
			} );
			
			/* HTML5 attribute detection - build an mData object automatically if the
			 * attributes are found
			 */
			if ( rowOne.length ) {
				var a = function ( cell, name ) {
					return cell.getAttribute( 'data-'+name ) !== null ? name : null;
				};
			
				$( rowOne[0] ).children('th, td').each( function (i, cell) {
					var col = oSettings.aoColumns[i];
			
					if ( col.mData === i ) {
						var sort = a( cell, 'sort' ) || a( cell, 'order' );
						var filter = a( cell, 'filter' ) || a( cell, 'search' );
			
						if ( sort !== null || filter !== null ) {
							col.mData = {
								_:      i+'.display',
								sort:   sort !== null   ? i+'.@data-'+sort   : undefined,
								type:   sort !== null   ? i+'.@data-'+sort   : undefined,
								filter: filter !== null ? i+'.@data-'+filter : undefined
							};
			
							_fnColumnOptions( oSettings, i );
						}
					}
				} );
			}
			
			var features = oSettings.oFeatures;
			var loadedInit = function () {
				/*
				 * Sorting
				 * @todo For modularisation (1.11) this needs to do into a sort start up handler
				 */
			
				// If aaSorting is not defined, then we use the first indicator in asSorting
				// in case that has been altered, so the default sort reflects that option
				if ( oInit.aaSorting === undefined ) {
					var sorting = oSettings.aaSorting;
					for ( i=0, iLen=sorting.length ; i<iLen ; i++ ) {
						sorting[i][1] = oSettings.aoColumns[ i ].asSorting[0];
					}
				}
			
				/* Do a first pass on the sorting classes (allows any size changes to be taken into
				 * account, and also will apply sorting disabled classes if disabled
				 */
				_fnSortingClasses( oSettings );
			
				if ( features.bSort ) {
					_fnCallbackReg( oSettings, 'aoDrawCallback', function () {
						if ( oSettings.bSorted ) {
							var aSort = _fnSortFlatten( oSettings );
							var sortedColumns = {};
			
							$.each( aSort, function (i, val) {
								sortedColumns[ val.src ] = val.dir;
							} );
			
							_fnCallbackFire( oSettings, null, 'order', [oSettings, aSort, sortedColumns] );
							_fnSortAria( oSettings );
						}
					} );
				}
			
				_fnCallbackReg( oSettings, 'aoDrawCallback', function () {
					if ( oSettings.bSorted || _fnDataSource( oSettings ) === 'ssp' || features.bDeferRender ) {
						_fnSortingClasses( oSettings );
					}
				}, 'sc' );
			
			
				/*
				 * Final init
				 * Cache the header, body and footer as required, creating them if needed
				 */
			
				// Work around for Webkit bug 83867 - store the caption-side before removing from doc
				var captions = $this.children('caption').each( function () {
					this._captionSide = $(this).css('caption-side');
				} );
			
				var thead = $this.children('thead');
				if ( thead.length === 0 ) {
					thead = $('<thead/>').appendTo($this);
				}
				oSettings.nTHead = thead[0];
			
				var tbody = $this.children('tbody');
				if ( tbody.length === 0 ) {
					tbody = $('<tbody/>').appendTo($this);
				}
				oSettings.nTBody = tbody[0];
			
				var tfoot = $this.children('tfoot');
				if ( tfoot.length === 0 && captions.length > 0 && (oSettings.oScroll.sX !== "" || oSettings.oScroll.sY !== "") ) {
					// If we are a scrolling table, and no footer has been given, then we need to create
					// a tfoot element for the caption element to be appended to
					tfoot = $('<tfoot/>').appendTo($this);
				}
			
				if ( tfoot.length === 0 || tfoot.children().length === 0 ) {
					$this.addClass( oClasses.sNoFooter );
				}
				else if ( tfoot.length > 0 ) {
					oSettings.nTFoot = tfoot[0];
					_fnDetectHeader( oSettings.aoFooter, oSettings.nTFoot );
				}
			
				/* Check if there is data passing into the constructor */
				if ( oInit.aaData ) {
					for ( i=0 ; i<oInit.aaData.length ; i++ ) {
						_fnAddData( oSettings, oInit.aaData[ i ] );
					}
				}
				else if ( oSettings.bDeferLoading || _fnDataSource( oSettings ) == 'dom' ) {
					/* Grab the data from the page - only do this when deferred loading or no Ajax
					 * source since there is no point in reading the DOM data if we are then going
					 * to replace it with Ajax data
					 */
					_fnAddTr( oSettings, $(oSettings.nTBody).children('tr') );
				}
			
				/* Copy the data index array */
				oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
			
				/* Initialisation complete - table can be drawn */
				oSettings.bInitialised = true;
			
				/* Check if we need to initialise the table (it might not have been handed off to the
				 * language processor)
				 */
				if ( bInitHandedOff === false ) {
					_fnInitialise( oSettings );
				}
			};
			
			/* Must be done after everything which can be overridden by the state saving! */
			if ( oInit.bStateSave )
			{
				features.bStateSave = true;
				_fnCallbackReg( oSettings, 'aoDrawCallback', _fnSaveState, 'state_save' );
				_fnLoadState( oSettings, oInit, loadedInit );
			}
			else {
				loadedInit();
			}
			
		} );
		_that = null;
		return this;
	};

	
	/*
	 * It is useful to have variables which are scoped locally so only the
	 * DataTables functions can access them and they don't leak into global space.
	 * At the same time these functions are often useful over multiple files in the
	 * core and API, so we list, or at least document, all variables which are used
	 * by DataTables as private variables here. This also ensures that there is no
	 * clashing of variable names and that they can easily referenced for reuse.
	 */
	
	
	// Defined else where
	//  _selector_run
	//  _selector_opts
	//  _selector_first
	//  _selector_row_indexes
	
	var _ext; // DataTable.ext
	var _Api; // DataTable.Api
	var _api_register; // DataTable.Api.register
	var _api_registerPlural; // DataTable.Api.registerPlural
	
	var _re_dic = {};
	var _re_new_lines = /[\r\n]/g;
	var _re_html = /<.*?>/g;
	
	// This is not strict ISO8601 - Date.parse() is quite lax, although
	// implementations differ between browsers.
	var _re_date = /^\d{2,4}[\.\/\-]\d{1,2}[\.\/\-]\d{1,2}([T ]{1}\d{1,2}[:\.]\d{2}([\.:]\d{2})?)?$/;
	
	// Escape regular expression special characters
	var _re_escape_regex = new RegExp( '(\\' + [ '/', '.', '*', '+', '?', '|', '(', ')', '[', ']', '{', '}', '\\', '$', '^', '-' ].join('|\\') + ')', 'g' );
	
	// http://en.wikipedia.org/wiki/Foreign_exchange_market
	// - \u20BD - Russian ruble.
	// - \u20a9 - South Korean Won
	// - \u20BA - Turkish Lira
	// - \u20B9 - Indian Rupee
	// - R - Brazil (R$) and South Africa
	// - fr - Swiss Franc
	// - kr - Swedish krona, Norwegian krone and Danish krone
	// - \u2009 is thin space and \u202F is narrow no-break space, both used in many
	//   standards as thousands separators.
	var _re_formatted_numeric = /[',$£€¥%\u2009\u202F\u20BD\u20a9\u20BArfk]/gi;
	
	
	var _empty = function ( d ) {
		return !d || d === true || d === '-' ? true : false;
	};
	
	
	var _intVal = function ( s ) {
		var integer = parseInt( s, 10 );
		return !isNaN(integer) && isFinite(s) ? integer : null;
	};
	
	// Convert from a formatted number with characters other than `.` as the
	// decimal place, to a Javascript number
	var _numToDecimal = function ( num, decimalPoint ) {
		// Cache created regular expressions for speed as this function is called often
		if ( ! _re_dic[ decimalPoint ] ) {
			_re_dic[ decimalPoint ] = new RegExp( _fnEscapeRegex( decimalPoint ), 'g' );
		}
		return typeof num === 'string' && decimalPoint !== '.' ?
			num.replace( /\./g, '' ).replace( _re_dic[ decimalPoint ], '.' ) :
			num;
	};
	
	
	var _isNumber = function ( d, decimalPoint, formatted ) {
		var strType = typeof d === 'string';
	
		// If empty return immediately so there must be a number if it is a
		// formatted string (this stops the string "k", or "kr", etc being detected
		// as a formatted number for currency
		if ( _empty( d ) ) {
			return true;
		}
	
		if ( decimalPoint && strType ) {
			d = _numToDecimal( d, decimalPoint );
		}
	
		if ( formatted && strType ) {
			d = d.replace( _re_formatted_numeric, '' );
		}
	
		return !isNaN( parseFloat(d) ) && isFinite( d );
	};
	
	
	// A string without HTML in it can be considered to be HTML still
	var _isHtml = function ( d ) {
		return _empty( d ) || typeof d === 'string';
	};
	
	
	var _htmlNumeric = function ( d, decimalPoint, formatted ) {
		if ( _empty( d ) ) {
			return true;
		}
	
		var html = _isHtml( d );
		return ! html ?
			null :
			_isNumber( _stripHtml( d ), decimalPoint, formatted ) ?
				true :
				null;
	};
	
	
	var _pluck = function ( a, prop, prop2 ) {
		var out = [];
		var i=0, ien=a.length;
	
		// Could have the test in the loop for slightly smaller code, but speed
		// is essential here
		if ( prop2 !== undefined ) {
			for ( ; i<ien ; i++ ) {
				if ( a[i] && a[i][ prop ] ) {
					out.push( a[i][ prop ][ prop2 ] );
				}
			}
		}
		else {
			for ( ; i<ien ; i++ ) {
				if ( a[i] ) {
					out.push( a[i][ prop ] );
				}
			}
		}
	
		return out;
	};
	
	
	// Basically the same as _pluck, but rather than looping over `a` we use `order`
	// as the indexes to pick from `a`
	var _pluck_order = function ( a, order, prop, prop2 )
	{
		var out = [];
		var i=0, ien=order.length;
	
		// Could have the test in the loop for slightly smaller code, but speed
		// is essential here
		if ( prop2 !== undefined ) {
			for ( ; i<ien ; i++ ) {
				if ( a[ order[i] ][ prop ] ) {
					out.push( a[ order[i] ][ prop ][ prop2 ] );
				}
			}
		}
		else {
			for ( ; i<ien ; i++ ) {
				out.push( a[ order[i] ][ prop ] );
			}
		}
	
		return out;
	};
	
	
	var _range = function ( len, start )
	{
		var out = [];
		var end;
	
		if ( start === undefined ) {
			start = 0;
			end = len;
		}
		else {
			end = start;
			start = len;
		}
	
		for ( var i=start ; i<end ; i++ ) {
			out.push( i );
		}
	
		return out;
	};
	
	
	var _removeEmpty = function ( a )
	{
		var out = [];
	
		for ( var i=0, ien=a.length ; i<ien ; i++ ) {
			if ( a[i] ) { // careful - will remove all falsy values!
				out.push( a[i] );
			}
		}
	
		return out;
	};
	
	
	var _stripHtml = function ( d ) {
		return d.replace( _re_html, '' );
	};
	
	
	/**
	 * Determine if all values in the array are unique. This means we can short
	 * cut the _unique method at the cost of a single loop. A sorted array is used
	 * to easily check the values.
	 *
	 * @param  {array} src Source array
	 * @return {boolean} true if all unique, false otherwise
	 * @ignore
	 */
	var _areAllUnique = function ( src ) {
		if ( src.length < 2 ) {
			return true;
		}
	
		var sorted = src.slice().sort();
		var last = sorted[0];
	
		for ( var i=1, ien=sorted.length ; i<ien ; i++ ) {
			if ( sorted[i] === last ) {
				return false;
			}
	
			last = sorted[i];
		}
	
		return true;
	};
	
	
	/**
	 * Find the unique elements in a source array.
	 *
	 * @param  {array} src Source array
	 * @return {array} Array of unique items
	 * @ignore
	 */
	var _unique = function ( src )
	{
		if ( _areAllUnique( src ) ) {
			return src.slice();
		}
	
		// A faster unique method is to use object keys to identify used values,
		// but this doesn't work with arrays or objects, which we must also
		// consider. See jsperf.com/compare-array-unique-versions/4 for more
		// information.
		var
			out = [],
			val,
			i, ien=src.length,
			j, k=0;
	
		again: for ( i=0 ; i<ien ; i++ ) {
			val = src[i];
	
			for ( j=0 ; j<k ; j++ ) {
				if ( out[j] === val ) {
					continue again;
				}
			}
	
			out.push( val );
			k++;
		}
	
		return out;
	};
	
	
	/**
	 * DataTables utility methods
	 * 
	 * This namespace provides helper methods that DataTables uses internally to
	 * create a DataTable, but which are not exclusively used only for DataTables.
	 * These methods can be used by extension authors to save the duplication of
	 * code.
	 *
	 *  @namespace
	 */
	DataTable.util = {
		/**
		 * Throttle the calls to a function. Arguments and context are maintained
		 * for the throttled function.
		 *
		 * @param {function} fn Function to be called
		 * @param {integer} freq Call frequency in mS
		 * @return {function} Wrapped function
		 */
		throttle: function ( fn, freq ) {
			var
				frequency = freq !== undefined ? freq : 200,
				last,
				timer;
	
			return function () {
				var
					that = this,
					now  = +new Date(),
					args = arguments;
	
				if ( last && now < last + frequency ) {
					clearTimeout( timer );
	
					timer = setTimeout( function () {
						last = undefined;
						fn.apply( that, args );
					}, frequency );
				}
				else {
					last = now;
					fn.apply( that, args );
				}
			};
		},
	
	
		/**
		 * Escape a string such that it can be used in a regular expression
		 *
		 *  @param {string} val string to escape
		 *  @returns {string} escaped string
		 */
		escapeRegex: function ( val ) {
			return val.replace( _re_escape_regex, '\\$1' );
		}
	};
	
	
	
	/**
	 * Create a mapping object that allows camel case parameters to be looked up
	 * for their Hungarian counterparts. The mapping is stored in a private
	 * parameter called `_hungarianMap` which can be accessed on the source object.
	 *  @param {object} o
	 *  @memberof DataTable#oApi
	 */
	function _fnHungarianMap ( o )
	{
		var
			hungarian = 'a aa ai ao as b fn i m o s ',
			match,
			newKey,
			map = {};
	
		$.each( o, function (key, val) {
			match = key.match(/^([^A-Z]+?)([A-Z])/);
	
			if ( match && hungarian.indexOf(match[1]+' ') !== -1 )
			{
				newKey = key.replace( match[0], match[2].toLowerCase() );
				map[ newKey ] = key;
	
				if ( match[1] === 'o' )
				{
					_fnHungarianMap( o[key] );
				}
			}
		} );
	
		o._hungarianMap = map;
	}
	
	
	/**
	 * Convert from camel case parameters to Hungarian, based on a Hungarian map
	 * created by _fnHungarianMap.
	 *  @param {object} src The model object which holds all parameters that can be
	 *    mapped.
	 *  @param {object} user The object to convert from camel case to Hungarian.
	 *  @param {boolean} force When set to `true`, properties which already have a
	 *    Hungarian value in the `user` object will be overwritten. Otherwise they
	 *    won't be.
	 *  @memberof DataTable#oApi
	 */
	function _fnCamelToHungarian ( src, user, force )
	{
		if ( ! src._hungarianMap ) {
			_fnHungarianMap( src );
		}
	
		var hungarianKey;
	
		$.each( user, function (key, val) {
			hungarianKey = src._hungarianMap[ key ];
	
			if ( hungarianKey !== undefined && (force || user[hungarianKey] === undefined) )
			{
				// For objects, we need to buzz down into the object to copy parameters
				if ( hungarianKey.charAt(0) === 'o' )
				{
					// Copy the camelCase options over to the hungarian
					if ( ! user[ hungarianKey ] ) {
						user[ hungarianKey ] = {};
					}
					$.extend( true, user[hungarianKey], user[key] );
	
					_fnCamelToHungarian( src[hungarianKey], user[hungarianKey], force );
				}
				else {
					user[hungarianKey] = user[ key ];
				}
			}
		} );
	}
	
	
	/**
	 * Language compatibility - when certain options are given, and others aren't, we
	 * need to duplicate the values over, in order to provide backwards compatibility
	 * with older language files.
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnLanguageCompat( lang )
	{
		var defaults = DataTable.defaults.oLanguage;
		var zeroRecords = lang.sZeroRecords;
	
		/* Backwards compatibility - if there is no sEmptyTable given, then use the same as
		 * sZeroRecords - assuming that is given.
		 */
		if ( ! lang.sEmptyTable && zeroRecords &&
			defaults.sEmptyTable === "No data available in table" )
		{
			_fnMap( lang, lang, 'sZeroRecords', 'sEmptyTable' );
		}
	
		/* Likewise with loading records */
		if ( ! lang.sLoadingRecords && zeroRecords &&
			defaults.sLoadingRecords === "Loading..." )
		{
			_fnMap( lang, lang, 'sZeroRecords', 'sLoadingRecords' );
		}
	
		// Old parameter name of the thousands separator mapped onto the new
		if ( lang.sInfoThousands ) {
			lang.sThousands = lang.sInfoThousands;
		}
	
		var decimal = lang.sDecimal;
		if ( decimal ) {
			_addNumericSort( decimal );
		}
	}
	
	
	/**
	 * Map one parameter onto another
	 *  @param {object} o Object to map
	 *  @param {*} knew The new parameter name
	 *  @param {*} old The old parameter name
	 */
	var _fnCompatMap = function ( o, knew, old ) {
		if ( o[ knew ] !== undefined ) {
			o[ old ] = o[ knew ];
		}
	};
	
	
	/**
	 * Provide backwards compatibility for the main DT options. Note that the new
	 * options are mapped onto the old parameters, so this is an external interface
	 * change only.
	 *  @param {object} init Object to map
	 */
	function _fnCompatOpts ( init )
	{
		_fnCompatMap( init, 'ordering',      'bSort' );
		_fnCompatMap( init, 'orderMulti',    'bSortMulti' );
		_fnCompatMap( init, 'orderClasses',  'bSortClasses' );
		_fnCompatMap( init, 'orderCellsTop', 'bSortCellsTop' );
		_fnCompatMap( init, 'order',         'aaSorting' );
		_fnCompatMap( init, 'orderFixed',    'aaSortingFixed' );
		_fnCompatMap( init, 'paging',        'bPaginate' );
		_fnCompatMap( init, 'pagingType',    'sPaginationType' );
		_fnCompatMap( init, 'pageLength',    'iDisplayLength' );
		_fnCompatMap( init, 'searching',     'bFilter' );
	
		// Boolean initialisation of x-scrolling
		if ( typeof init.sScrollX === 'boolean' ) {
			init.sScrollX = init.sScrollX ? '100%' : '';
		}
		if ( typeof init.scrollX === 'boolean' ) {
			init.scrollX = init.scrollX ? '100%' : '';
		}
	
		// Column search objects are in an array, so it needs to be converted
		// element by element
		var searchCols = init.aoSearchCols;
	
		if ( searchCols ) {
			for ( var i=0, ien=searchCols.length ; i<ien ; i++ ) {
				if ( searchCols[i] ) {
					_fnCamelToHungarian( DataTable.models.oSearch, searchCols[i] );
				}
			}
		}
	}
	
	
	/**
	 * Provide backwards compatibility for column options. Note that the new options
	 * are mapped onto the old parameters, so this is an external interface change
	 * only.
	 *  @param {object} init Object to map
	 */
	function _fnCompatCols ( init )
	{
		_fnCompatMap( init, 'orderable',     'bSortable' );
		_fnCompatMap( init, 'orderData',     'aDataSort' );
		_fnCompatMap( init, 'orderSequence', 'asSorting' );
		_fnCompatMap( init, 'orderDataType', 'sortDataType' );
	
		// orderData can be given as an integer
		var dataSort = init.aDataSort;
		if ( typeof dataSort === 'number' && ! $.isArray( dataSort ) ) {
			init.aDataSort = [ dataSort ];
		}
	}
	
	
	/**
	 * Browser feature detection for capabilities, quirks
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnBrowserDetect( settings )
	{
		// We don't need to do this every time DataTables is constructed, the values
		// calculated are specific to the browser and OS configuration which we
		// don't expect to change between initialisations
		if ( ! DataTable.__browser ) {
			var browser = {};
			DataTable.__browser = browser;
	
			// Scrolling feature / quirks detection
			var n = $('<div/>')
				.css( {
					position: 'fixed',
					top: 0,
					left: $(window).scrollLeft()*-1, // allow for scrolling
					height: 1,
					width: 1,
					overflow: 'hidden'
				} )
				.append(
					$('<div/>')
						.css( {
							position: 'absolute',
							top: 1,
							left: 1,
							width: 100,
							overflow: 'scroll'
						} )
						.append(
							$('<div/>')
								.css( {
									width: '100%',
									height: 10
								} )
						)
				)
				.appendTo( 'body' );
	
			var outer = n.children();
			var inner = outer.children();
	
			// Numbers below, in order, are:
			// inner.offsetWidth, inner.clientWidth, outer.offsetWidth, outer.clientWidth
			//
			// IE6 XP:                           100 100 100  83
			// IE7 Vista:                        100 100 100  83
			// IE 8+ Windows:                     83  83 100  83
			// Evergreen Windows:                 83  83 100  83
			// Evergreen Mac with scrollbars:     85  85 100  85
			// Evergreen Mac without scrollbars: 100 100 100 100
	
			// Get scrollbar width
			browser.barWidth = outer[0].offsetWidth - outer[0].clientWidth;
	
			// IE6/7 will oversize a width 100% element inside a scrolling element, to
			// include the width of the scrollbar, while other browsers ensure the inner
			// element is contained without forcing scrolling
			browser.bScrollOversize = inner[0].offsetWidth === 100 && outer[0].clientWidth !== 100;
	
			// In rtl text layout, some browsers (most, but not all) will place the
			// scrollbar on the left, rather than the right.
			browser.bScrollbarLeft = Math.round( inner.offset().left ) !== 1;
	
			// IE8- don't provide height and width for getBoundingClientRect
			browser.bBounding = n[0].getBoundingClientRect().width ? true : false;
	
			n.remove();
		}
	
		$.extend( settings.oBrowser, DataTable.__browser );
		settings.oScroll.iBarWidth = DataTable.__browser.barWidth;
	}
	
	
	/**
	 * Array.prototype reduce[Right] method, used for browsers which don't support
	 * JS 1.6. Done this way to reduce code size, since we iterate either way
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnReduce ( that, fn, init, start, end, inc )
	{
		var
			i = start,
			value,
			isSet = false;
	
		if ( init !== undefined ) {
			value = init;
			isSet = true;
		}
	
		while ( i !== end ) {
			if ( ! that.hasOwnProperty(i) ) {
				continue;
			}
	
			value = isSet ?
				fn( value, that[i], i, that ) :
				that[i];
	
			isSet = true;
			i += inc;
		}
	
		return value;
	}
	
	/**
	 * Add a column to the list used for the table with default values
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} nTh The th element for this column
	 *  @memberof DataTable#oApi
	 */
	function _fnAddColumn( oSettings, nTh )
	{
		// Add column to aoColumns array
		var oDefaults = DataTable.defaults.column;
		var iCol = oSettings.aoColumns.length;
		var oCol = $.extend( {}, DataTable.models.oColumn, oDefaults, {
			"nTh": nTh ? nTh : document.createElement('th'),
			"sTitle":    oDefaults.sTitle    ? oDefaults.sTitle    : nTh ? nTh.innerHTML : '',
			"aDataSort": oDefaults.aDataSort ? oDefaults.aDataSort : [iCol],
			"mData": oDefaults.mData ? oDefaults.mData : iCol,
			idx: iCol
		} );
		oSettings.aoColumns.push( oCol );
	
		// Add search object for column specific search. Note that the `searchCols[ iCol ]`
		// passed into extend can be undefined. This allows the user to give a default
		// with only some of the parameters defined, and also not give a default
		var searchCols = oSettings.aoPreSearchCols;
		searchCols[ iCol ] = $.extend( {}, DataTable.models.oSearch, searchCols[ iCol ] );
	
		// Use the default column options function to initialise classes etc
		_fnColumnOptions( oSettings, iCol, $(nTh).data() );
	}
	
	
	/**
	 * Apply options for a column
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iCol column index to consider
	 *  @param {object} oOptions object with sType, bVisible and bSearchable etc
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnOptions( oSettings, iCol, oOptions )
	{
		var oCol = oSettings.aoColumns[ iCol ];
		var oClasses = oSettings.oClasses;
		var th = $(oCol.nTh);
	
		// Try to get width information from the DOM. We can't get it from CSS
		// as we'd need to parse the CSS stylesheet. `width` option can override
		if ( ! oCol.sWidthOrig ) {
			// Width attribute
			oCol.sWidthOrig = th.attr('width') || null;
	
			// Style attribute
			var t = (th.attr('style') || '').match(/width:\s*(\d+[pxem%]+)/);
			if ( t ) {
				oCol.sWidthOrig = t[1];
			}
		}
	
		/* User specified column options */
		if ( oOptions !== undefined && oOptions !== null )
		{
			// Backwards compatibility
			_fnCompatCols( oOptions );
	
			// Map camel case parameters to their Hungarian counterparts
			_fnCamelToHungarian( DataTable.defaults.column, oOptions );
	
			/* Backwards compatibility for mDataProp */
			if ( oOptions.mDataProp !== undefined && !oOptions.mData )
			{
				oOptions.mData = oOptions.mDataProp;
			}
	
			if ( oOptions.sType )
			{
				oCol._sManualType = oOptions.sType;
			}
	
			// `class` is a reserved word in Javascript, so we need to provide
			// the ability to use a valid name for the camel case input
			if ( oOptions.className && ! oOptions.sClass )
			{
				oOptions.sClass = oOptions.className;
			}
			if ( oOptions.sClass ) {
				th.addClass( oOptions.sClass );
			}
	
			$.extend( oCol, oOptions );
			_fnMap( oCol, oOptions, "sWidth", "sWidthOrig" );
	
			/* iDataSort to be applied (backwards compatibility), but aDataSort will take
			 * priority if defined
			 */
			if ( oOptions.iDataSort !== undefined )
			{
				oCol.aDataSort = [ oOptions.iDataSort ];
			}
			_fnMap( oCol, oOptions, "aDataSort" );
		}
	
		/* Cache the data get and set functions for speed */
		var mDataSrc = oCol.mData;
		var mData = _fnGetObjectDataFn( mDataSrc );
		var mRender = oCol.mRender ? _fnGetObjectDataFn( oCol.mRender ) : null;
	
		var attrTest = function( src ) {
			return typeof src === 'string' && src.indexOf('@') !== -1;
		};
		oCol._bAttrSrc = $.isPlainObject( mDataSrc ) && (
			attrTest(mDataSrc.sort) || attrTest(mDataSrc.type) || attrTest(mDataSrc.filter)
		);
		oCol._setter = null;
	
		oCol.fnGetData = function (rowData, type, meta) {
			var innerData = mData( rowData, type, undefined, meta );
	
			return mRender && type ?
				mRender( innerData, type, rowData, meta ) :
				innerData;
		};
		oCol.fnSetData = function ( rowData, val, meta ) {
			return _fnSetObjectDataFn( mDataSrc )( rowData, val, meta );
		};
	
		// Indicate if DataTables should read DOM data as an object or array
		// Used in _fnGetRowElements
		if ( typeof mDataSrc !== 'number' ) {
			oSettings._rowReadObject = true;
		}
	
		/* Feature sorting overrides column specific when off */
		if ( !oSettings.oFeatures.bSort )
		{
			oCol.bSortable = false;
			th.addClass( oClasses.sSortableNone ); // Have to add class here as order event isn't called
		}
	
		/* Check that the class assignment is correct for sorting */
		var bAsc = $.inArray('asc', oCol.asSorting) !== -1;
		var bDesc = $.inArray('desc', oCol.asSorting) !== -1;
		if ( !oCol.bSortable || (!bAsc && !bDesc) )
		{
			oCol.sSortingClass = oClasses.sSortableNone;
			oCol.sSortingClassJUI = "";
		}
		else if ( bAsc && !bDesc )
		{
			oCol.sSortingClass = oClasses.sSortableAsc;
			oCol.sSortingClassJUI = oClasses.sSortJUIAscAllowed;
		}
		else if ( !bAsc && bDesc )
		{
			oCol.sSortingClass = oClasses.sSortableDesc;
			oCol.sSortingClassJUI = oClasses.sSortJUIDescAllowed;
		}
		else
		{
			oCol.sSortingClass = oClasses.sSortable;
			oCol.sSortingClassJUI = oClasses.sSortJUI;
		}
	}
	
	
	/**
	 * Adjust the table column widths for new data. Note: you would probably want to
	 * do a redraw after calling this function!
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnAdjustColumnSizing ( settings )
	{
		/* Not interested in doing column width calculation if auto-width is disabled */
		if ( settings.oFeatures.bAutoWidth !== false )
		{
			var columns = settings.aoColumns;
	
			_fnCalculateColumnWidths( settings );
			for ( var i=0 , iLen=columns.length ; i<iLen ; i++ )
			{
				columns[i].nTh.style.width = columns[i].sWidth;
			}
		}
	
		var scroll = settings.oScroll;
		if ( scroll.sY !== '' || scroll.sX !== '')
		{
			_fnScrollDraw( settings );
		}
	
		_fnCallbackFire( settings, null, 'column-sizing', [settings] );
	}
	
	
	/**
	 * Covert the index of a visible column to the index in the data array (take account
	 * of hidden columns)
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iMatch Visible column index to lookup
	 *  @returns {int} i the data index
	 *  @memberof DataTable#oApi
	 */
	function _fnVisibleToColumnIndex( oSettings, iMatch )
	{
		var aiVis = _fnGetColumns( oSettings, 'bVisible' );
	
		return typeof aiVis[iMatch] === 'number' ?
			aiVis[iMatch] :
			null;
	}
	
	
	/**
	 * Covert the index of an index in the data array and convert it to the visible
	 *   column index (take account of hidden columns)
	 *  @param {int} iMatch Column index to lookup
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {int} i the data index
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnIndexToVisible( oSettings, iMatch )
	{
		var aiVis = _fnGetColumns( oSettings, 'bVisible' );
		var iPos = $.inArray( iMatch, aiVis );
	
		return iPos !== -1 ? iPos : null;
	}
	
	
	/**
	 * Get the number of visible columns
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {int} i the number of visible columns
	 *  @memberof DataTable#oApi
	 */
	function _fnVisbleColumns( oSettings )
	{
		var vis = 0;
	
		// No reduce in IE8, use a loop for now
		$.each( oSettings.aoColumns, function ( i, col ) {
			if ( col.bVisible && $(col.nTh).css('display') !== 'none' ) {
				vis++;
			}
		} );
	
		return vis;
	}
	
	
	/**
	 * Get an array of column indexes that match a given property
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sParam Parameter in aoColumns to look for - typically
	 *    bVisible or bSearchable
	 *  @returns {array} Array of indexes with matched properties
	 *  @memberof DataTable#oApi
	 */
	function _fnGetColumns( oSettings, sParam )
	{
		var a = [];
	
		$.map( oSettings.aoColumns, function(val, i) {
			if ( val[sParam] ) {
				a.push( i );
			}
		} );
	
		return a;
	}
	
	
	/**
	 * Calculate the 'type' of a column
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnColumnTypes ( settings )
	{
		var columns = settings.aoColumns;
		var data = settings.aoData;
		var types = DataTable.ext.type.detect;
		var i, ien, j, jen, k, ken;
		var col, cell, detectedType, cache;
	
		// For each column, spin over the 
		for ( i=0, ien=columns.length ; i<ien ; i++ ) {
			col = columns[i];
			cache = [];
	
			if ( ! col.sType && col._sManualType ) {
				col.sType = col._sManualType;
			}
			else if ( ! col.sType ) {
				for ( j=0, jen=types.length ; j<jen ; j++ ) {
					for ( k=0, ken=data.length ; k<ken ; k++ ) {
						// Use a cache array so we only need to get the type data
						// from the formatter once (when using multiple detectors)
						if ( cache[k] === undefined ) {
							cache[k] = _fnGetCellData( settings, k, i, 'type' );
						}
	
						detectedType = types[j]( cache[k], settings );
	
						// If null, then this type can't apply to this column, so
						// rather than testing all cells, break out. There is an
						// exception for the last type which is `html`. We need to
						// scan all rows since it is possible to mix string and HTML
						// types
						if ( ! detectedType && j !== types.length-1 ) {
							break;
						}
	
						// Only a single match is needed for html type since it is
						// bottom of the pile and very similar to string
						if ( detectedType === 'html' ) {
							break;
						}
					}
	
					// Type is valid for all data points in the column - use this
					// type
					if ( detectedType ) {
						col.sType = detectedType;
						break;
					}
				}
	
				// Fall back - if no type was detected, always use string
				if ( ! col.sType ) {
					col.sType = 'string';
				}
			}
		}
	}
	
	
	/**
	 * Take the column definitions and static columns arrays and calculate how
	 * they relate to column indexes. The callback function will then apply the
	 * definition found for a column to a suitable configuration object.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {array} aoColDefs The aoColumnDefs array that is to be applied
	 *  @param {array} aoCols The aoColumns array that defines columns individually
	 *  @param {function} fn Callback function - takes two parameters, the calculated
	 *    column index and the definition for that column.
	 *  @memberof DataTable#oApi
	 */
	function _fnApplyColumnDefs( oSettings, aoColDefs, aoCols, fn )
	{
		var i, iLen, j, jLen, k, kLen, def;
		var columns = oSettings.aoColumns;
	
		// Column definitions with aTargets
		if ( aoColDefs )
		{
			/* Loop over the definitions array - loop in reverse so first instance has priority */
			for ( i=aoColDefs.length-1 ; i>=0 ; i-- )
			{
				def = aoColDefs[i];
	
				/* Each definition can target multiple columns, as it is an array */
				var aTargets = def.targets !== undefined ?
					def.targets :
					def.aTargets;
	
				if ( ! $.isArray( aTargets ) )
				{
					aTargets = [ aTargets ];
				}
	
				for ( j=0, jLen=aTargets.length ; j<jLen ; j++ )
				{
					if ( typeof aTargets[j] === 'number' && aTargets[j] >= 0 )
					{
						/* Add columns that we don't yet know about */
						while( columns.length <= aTargets[j] )
						{
							_fnAddColumn( oSettings );
						}
	
						/* Integer, basic index */
						fn( aTargets[j], def );
					}
					else if ( typeof aTargets[j] === 'number' && aTargets[j] < 0 )
					{
						/* Negative integer, right to left column counting */
						fn( columns.length+aTargets[j], def );
					}
					else if ( typeof aTargets[j] === 'string' )
					{
						/* Class name matching on TH element */
						for ( k=0, kLen=columns.length ; k<kLen ; k++ )
						{
							if ( aTargets[j] == "_all" ||
							     $(columns[k].nTh).hasClass( aTargets[j] ) )
							{
								fn( k, def );
							}
						}
					}
				}
			}
		}
	
		// Statically defined columns array
		if ( aoCols )
		{
			for ( i=0, iLen=aoCols.length ; i<iLen ; i++ )
			{
				fn( i, aoCols[i] );
			}
		}
	}
	
	/**
	 * Add a data array to the table, creating DOM node etc. This is the parallel to
	 * _fnGatherData, but for adding rows from a Javascript source, rather than a
	 * DOM source.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {array} aData data array to be added
	 *  @param {node} [nTr] TR element to add to the table - optional. If not given,
	 *    DataTables will create a row automatically
	 *  @param {array} [anTds] Array of TD|TH elements for the row - must be given
	 *    if nTr is.
	 *  @returns {int} >=0 if successful (index of new aoData entry), -1 if failed
	 *  @memberof DataTable#oApi
	 */
	function _fnAddData ( oSettings, aDataIn, nTr, anTds )
	{
		/* Create the object for storing information about this new row */
		var iRow = oSettings.aoData.length;
		var oData = $.extend( true, {}, DataTable.models.oRow, {
			src: nTr ? 'dom' : 'data',
			idx: iRow
		} );
	
		oData._aData = aDataIn;
		oSettings.aoData.push( oData );
	
		/* Create the cells */
		var nTd, sThisType;
		var columns = oSettings.aoColumns;
	
		// Invalidate the column types as the new data needs to be revalidated
		for ( var i=0, iLen=columns.length ; i<iLen ; i++ )
		{
			columns[i].sType = null;
		}
	
		/* Add to the display array */
		oSettings.aiDisplayMaster.push( iRow );
	
		var id = oSettings.rowIdFn( aDataIn );
		if ( id !== undefined ) {
			oSettings.aIds[ id ] = oData;
		}
	
		/* Create the DOM information, or register it if already present */
		if ( nTr || ! oSettings.oFeatures.bDeferRender )
		{
			_fnCreateTr( oSettings, iRow, nTr, anTds );
		}
	
		return iRow;
	}
	
	
	/**
	 * Add one or more TR elements to the table. Generally we'd expect to
	 * use this for reading data from a DOM sourced table, but it could be
	 * used for an TR element. Note that if a TR is given, it is used (i.e.
	 * it is not cloned).
	 *  @param {object} settings dataTables settings object
	 *  @param {array|node|jQuery} trs The TR element(s) to add to the table
	 *  @returns {array} Array of indexes for the added rows
	 *  @memberof DataTable#oApi
	 */
	function _fnAddTr( settings, trs )
	{
		var row;
	
		// Allow an individual node to be passed in
		if ( ! (trs instanceof $) ) {
			trs = $(trs);
		}
	
		return trs.map( function (i, el) {
			row = _fnGetRowElements( settings, el );
			return _fnAddData( settings, row.data, el, row.cells );
		} );
	}
	
	
	/**
	 * Take a TR element and convert it to an index in aoData
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} n the TR element to find
	 *  @returns {int} index if the node is found, null if not
	 *  @memberof DataTable#oApi
	 */
	function _fnNodeToDataIndex( oSettings, n )
	{
		return (n._DT_RowIndex!==undefined) ? n._DT_RowIndex : null;
	}
	
	
	/**
	 * Take a TD element and convert it into a column data index (not the visible index)
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iRow The row number the TD/TH can be found in
	 *  @param {node} n The TD/TH element to find
	 *  @returns {int} index if the node is found, -1 if not
	 *  @memberof DataTable#oApi
	 */
	function _fnNodeToColumnIndex( oSettings, iRow, n )
	{
		return $.inArray( n, oSettings.aoData[ iRow ].anCells );
	}
	
	
	/**
	 * Get the data for a given cell from the internal cache, taking into account data mapping
	 *  @param {object} settings dataTables settings object
	 *  @param {int} rowIdx aoData row id
	 *  @param {int} colIdx Column index
	 *  @param {string} type data get type ('display', 'type' 'filter' 'sort')
	 *  @returns {*} Cell data
	 *  @memberof DataTable#oApi
	 */
	function _fnGetCellData( settings, rowIdx, colIdx, type )
	{
		var draw           = settings.iDraw;
		var col            = settings.aoColumns[colIdx];
		var rowData        = settings.aoData[rowIdx]._aData;
		var defaultContent = col.sDefaultContent;
		var cellData       = col.fnGetData( rowData, type, {
			settings: settings,
			row:      rowIdx,
			col:      colIdx
		} );
	
		if ( cellData === undefined ) {
			if ( settings.iDrawError != draw && defaultContent === null ) {
				_fnLog( settings, 0, "Requested unknown parameter "+
					(typeof col.mData=='function' ? '{function}' : "'"+col.mData+"'")+
					" for row "+rowIdx+", column "+colIdx, 4 );
				settings.iDrawError = draw;
			}
			return defaultContent;
		}
	
		// When the data source is null and a specific data type is requested (i.e.
		// not the original data), we can use default column data
		if ( (cellData === rowData || cellData === null) && defaultContent !== null && type !== undefined ) {
			cellData = defaultContent;
		}
		else if ( typeof cellData === 'function' ) {
			// If the data source is a function, then we run it and use the return,
			// executing in the scope of the data object (for instances)
			return cellData.call( rowData );
		}
	
		if ( cellData === null && type == 'display' ) {
			return '';
		}
		return cellData;
	}
	
	
	/**
	 * Set the value for a specific cell, into the internal data cache
	 *  @param {object} settings dataTables settings object
	 *  @param {int} rowIdx aoData row id
	 *  @param {int} colIdx Column index
	 *  @param {*} val Value to set
	 *  @memberof DataTable#oApi
	 */
	function _fnSetCellData( settings, rowIdx, colIdx, val )
	{
		var col     = settings.aoColumns[colIdx];
		var rowData = settings.aoData[rowIdx]._aData;
	
		col.fnSetData( rowData, val, {
			settings: settings,
			row:      rowIdx,
			col:      colIdx
		}  );
	}
	
	
	// Private variable that is used to match action syntax in the data property object
	var __reArray = /\[.*?\]$/;
	var __reFn = /\(\)$/;
	
	/**
	 * Split string on periods, taking into account escaped periods
	 * @param  {string} str String to split
	 * @return {array} Split string
	 */
	function _fnSplitObjNotation( str )
	{
		return $.map( str.match(/(\\.|[^\.])+/g) || [''], function ( s ) {
			return s.replace(/\\\./g, '.');
		} );
	}
	
	
	/**
	 * Return a function that can be used to get data from a source object, taking
	 * into account the ability to use nested objects as a source
	 *  @param {string|int|function} mSource The data source for the object
	 *  @returns {function} Data get function
	 *  @memberof DataTable#oApi
	 */
	function _fnGetObjectDataFn( mSource )
	{
		if ( $.isPlainObject( mSource ) )
		{
			/* Build an object of get functions, and wrap them in a single call */
			var o = {};
			$.each( mSource, function (key, val) {
				if ( val ) {
					o[key] = _fnGetObjectDataFn( val );
				}
			} );
	
			return function (data, type, row, meta) {
				var t = o[type] || o._;
				return t !== undefined ?
					t(data, type, row, meta) :
					data;
			};
		}
		else if ( mSource === null )
		{
			/* Give an empty string for rendering / sorting etc */
			return function (data) { // type, row and meta also passed, but not used
				return data;
			};
		}
		else if ( typeof mSource === 'function' )
		{
			return function (data, type, row, meta) {
				return mSource( data, type, row, meta );
			};
		}
		else if ( typeof mSource === 'string' && (mSource.indexOf('.') !== -1 ||
			      mSource.indexOf('[') !== -1 || mSource.indexOf('(') !== -1) )
		{
			/* If there is a . in the source string then the data source is in a
			 * nested object so we loop over the data for each level to get the next
			 * level down. On each loop we test for undefined, and if found immediately
			 * return. This allows entire objects to be missing and sDefaultContent to
			 * be used if defined, rather than throwing an error
			 */
			var fetchData = function (data, type, src) {
				var arrayNotation, funcNotation, out, innerSrc;
	
				if ( src !== "" )
				{
					var a = _fnSplitObjNotation( src );
	
					for ( var i=0, iLen=a.length ; i<iLen ; i++ )
					{
						// Check if we are dealing with special notation
						arrayNotation = a[i].match(__reArray);
						funcNotation = a[i].match(__reFn);
	
						if ( arrayNotation )
						{
							// Array notation
							a[i] = a[i].replace(__reArray, '');
	
							// Condition allows simply [] to be passed in
							if ( a[i] !== "" ) {
								data = data[ a[i] ];
							}
							out = [];
	
							// Get the remainder of the nested object to get
							a.splice( 0, i+1 );
							innerSrc = a.join('.');
	
							// Traverse each entry in the array getting the properties requested
							if ( $.isArray( data ) ) {
								for ( var j=0, jLen=data.length ; j<jLen ; j++ ) {
									out.push( fetchData( data[j], type, innerSrc ) );
								}
							}
	
							// If a string is given in between the array notation indicators, that
							// is used to join the strings together, otherwise an array is returned
							var join = arrayNotation[0].substring(1, arrayNotation[0].length-1);
							data = (join==="") ? out : out.join(join);
	
							// The inner call to fetchData has already traversed through the remainder
							// of the source requested, so we exit from the loop
							break;
						}
						else if ( funcNotation )
						{
							// Function call
							a[i] = a[i].replace(__reFn, '');
							data = data[ a[i] ]();
							continue;
						}
	
						if ( data === null || data[ a[i] ] === undefined )
						{
							return undefined;
						}
						data = data[ a[i] ];
					}
				}
	
				return data;
			};
	
			return function (data, type) { // row and meta also passed, but not used
				return fetchData( data, type, mSource );
			};
		}
		else
		{
			/* Array or flat object mapping */
			return function (data, type) { // row and meta also passed, but not used
				return data[mSource];
			};
		}
	}
	
	
	/**
	 * Return a function that can be used to set data from a source object, taking
	 * into account the ability to use nested objects as a source
	 *  @param {string|int|function} mSource The data source for the object
	 *  @returns {function} Data set function
	 *  @memberof DataTable#oApi
	 */
	function _fnSetObjectDataFn( mSource )
	{
		if ( $.isPlainObject( mSource ) )
		{
			/* Unlike get, only the underscore (global) option is used for for
			 * setting data since we don't know the type here. This is why an object
			 * option is not documented for `mData` (which is read/write), but it is
			 * for `mRender` which is read only.
			 */
			return _fnSetObjectDataFn( mSource._ );
		}
		else if ( mSource === null )
		{
			/* Nothing to do when the data source is null */
			return function () {};
		}
		else if ( typeof mSource === 'function' )
		{
			return function (data, val, meta) {
				mSource( data, 'set', val, meta );
			};
		}
		else if ( typeof mSource === 'string' && (mSource.indexOf('.') !== -1 ||
			      mSource.indexOf('[') !== -1 || mSource.indexOf('(') !== -1) )
		{
			/* Like the get, we need to get data from a nested object */
			var setData = function (data, val, src) {
				var a = _fnSplitObjNotation( src ), b;
				var aLast = a[a.length-1];
				var arrayNotation, funcNotation, o, innerSrc;
	
				for ( var i=0, iLen=a.length-1 ; i<iLen ; i++ )
				{
					// Check if we are dealing with an array notation request
					arrayNotation = a[i].match(__reArray);
					funcNotation = a[i].match(__reFn);
	
					if ( arrayNotation )
					{
						a[i] = a[i].replace(__reArray, '');
						data[ a[i] ] = [];
	
						// Get the remainder of the nested object to set so we can recurse
						b = a.slice();
						b.splice( 0, i+1 );
						innerSrc = b.join('.');
	
						// Traverse each entry in the array setting the properties requested
						if ( $.isArray( val ) )
						{
							for ( var j=0, jLen=val.length ; j<jLen ; j++ )
							{
								o = {};
								setData( o, val[j], innerSrc );
								data[ a[i] ].push( o );
							}
						}
						else
						{
							// We've been asked to save data to an array, but it
							// isn't array data to be saved. Best that can be done
							// is to just save the value.
							data[ a[i] ] = val;
						}
	
						// The inner call to setData has already traversed through the remainder
						// of the source and has set the data, thus we can exit here
						return;
					}
					else if ( funcNotation )
					{
						// Function call
						a[i] = a[i].replace(__reFn, '');
						data = data[ a[i] ]( val );
					}
	
					// If the nested object doesn't currently exist - since we are
					// trying to set the value - create it
					if ( data[ a[i] ] === null || data[ a[i] ] === undefined )
					{
						data[ a[i] ] = {};
					}
					data = data[ a[i] ];
				}
	
				// Last item in the input - i.e, the actual set
				if ( aLast.match(__reFn ) )
				{
					// Function call
					data = data[ aLast.replace(__reFn, '') ]( val );
				}
				else
				{
					// If array notation is used, we just want to strip it and use the property name
					// and assign the value. If it isn't used, then we get the result we want anyway
					data[ aLast.replace(__reArray, '') ] = val;
				}
			};
	
			return function (data, val) { // meta is also passed in, but not used
				return setData( data, val, mSource );
			};
		}
		else
		{
			/* Array or flat object mapping */
			return function (data, val) { // meta is also passed in, but not used
				data[mSource] = val;
			};
		}
	}
	
	
	/**
	 * Return an array with the full table data
	 *  @param {object} oSettings dataTables settings object
	 *  @returns array {array} aData Master data array
	 *  @memberof DataTable#oApi
	 */
	function _fnGetDataMaster ( settings )
	{
		return _pluck( settings.aoData, '_aData' );
	}
	
	
	/**
	 * Nuke the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnClearTable( settings )
	{
		settings.aoData.length = 0;
		settings.aiDisplayMaster.length = 0;
		settings.aiDisplay.length = 0;
		settings.aIds = {};
	}
	
	
	 /**
	 * Take an array of integers (index array) and remove a target integer (value - not
	 * the key!)
	 *  @param {array} a Index array to target
	 *  @param {int} iTarget value to find
	 *  @memberof DataTable#oApi
	 */
	function _fnDeleteIndex( a, iTarget, splice )
	{
		var iTargetIndex = -1;
	
		for ( var i=0, iLen=a.length ; i<iLen ; i++ )
		{
			if ( a[i] == iTarget )
			{
				iTargetIndex = i;
			}
			else if ( a[i] > iTarget )
			{
				a[i]--;
			}
		}
	
		if ( iTargetIndex != -1 && splice === undefined )
		{
			a.splice( iTargetIndex, 1 );
		}
	}
	
	
	/**
	 * Mark cached data as invalid such that a re-read of the data will occur when
	 * the cached data is next requested. Also update from the data source object.
	 *
	 * @param {object} settings DataTables settings object
	 * @param {int}    rowIdx   Row index to invalidate
	 * @param {string} [src]    Source to invalidate from: undefined, 'auto', 'dom'
	 *     or 'data'
	 * @param {int}    [colIdx] Column index to invalidate. If undefined the whole
	 *     row will be invalidated
	 * @memberof DataTable#oApi
	 *
	 * @todo For the modularisation of v1.11 this will need to become a callback, so
	 *   the sort and filter methods can subscribe to it. That will required
	 *   initialisation options for sorting, which is why it is not already baked in
	 */
	function _fnInvalidate( settings, rowIdx, src, colIdx )
	{
		var row = settings.aoData[ rowIdx ];
		var i, ien;
		var cellWrite = function ( cell, col ) {
			// This is very frustrating, but in IE if you just write directly
			// to innerHTML, and elements that are overwritten are GC'ed,
			// even if there is a reference to them elsewhere
			while ( cell.childNodes.length ) {
				cell.removeChild( cell.firstChild );
			}
	
			cell.innerHTML = _fnGetCellData( settings, rowIdx, col, 'display' );
		};
	
		// Are we reading last data from DOM or the data object?
		if ( src === 'dom' || ((! src || src === 'auto') && row.src === 'dom') ) {
			// Read the data from the DOM
			row._aData = _fnGetRowElements(
					settings, row, colIdx, colIdx === undefined ? undefined : row._aData
				)
				.data;
		}
		else {
			// Reading from data object, update the DOM
			var cells = row.anCells;
	
			if ( cells ) {
				if ( colIdx !== undefined ) {
					cellWrite( cells[colIdx], colIdx );
				}
				else {
					for ( i=0, ien=cells.length ; i<ien ; i++ ) {
						cellWrite( cells[i], i );
					}
				}
			}
		}
	
		// For both row and cell invalidation, the cached data for sorting and
		// filtering is nulled out
		row._aSortData = null;
		row._aFilterData = null;
	
		// Invalidate the type for a specific column (if given) or all columns since
		// the data might have changed
		var cols = settings.aoColumns;
		if ( colIdx !== undefined ) {
			cols[ colIdx ].sType = null;
		}
		else {
			for ( i=0, ien=cols.length ; i<ien ; i++ ) {
				cols[i].sType = null;
			}
	
			// Update DataTables special `DT_*` attributes for the row
			_fnRowAttributes( settings, row );
		}
	}
	
	
	/**
	 * Build a data source object from an HTML row, reading the contents of the
	 * cells that are in the row.
	 *
	 * @param {object} settings DataTables settings object
	 * @param {node|object} TR element from which to read data or existing row
	 *   object from which to re-read the data from the cells
	 * @param {int} [colIdx] Optional column index
	 * @param {array|object} [d] Data source object. If `colIdx` is given then this
	 *   parameter should also be given and will be used to write the data into.
	 *   Only the column in question will be written
	 * @returns {object} Object with two parameters: `data` the data read, in
	 *   document order, and `cells` and array of nodes (they can be useful to the
	 *   caller, so rather than needing a second traversal to get them, just return
	 *   them from here).
	 * @memberof DataTable#oApi
	 */
	function _fnGetRowElements( settings, row, colIdx, d )
	{
		var
			tds = [],
			td = row.firstChild,
			name, col, o, i=0, contents,
			columns = settings.aoColumns,
			objectRead = settings._rowReadObject;
	
		// Allow the data object to be passed in, or construct
		d = d !== undefined ?
			d :
			objectRead ?
				{} :
				[];
	
		var attr = function ( str, td  ) {
			if ( typeof str === 'string' ) {
				var idx = str.indexOf('@');
	
				if ( idx !== -1 ) {
					var attr = str.substring( idx+1 );
					var setter = _fnSetObjectDataFn( str );
					setter( d, td.getAttribute( attr ) );
				}
			}
		};
	
		// Read data from a cell and store into the data object
		var cellProcess = function ( cell ) {
			if ( colIdx === undefined || colIdx === i ) {
				col = columns[i];
				contents = $.trim(cell.innerHTML);
	
				if ( col && col._bAttrSrc ) {
					var setter = _fnSetObjectDataFn( col.mData._ );
					setter( d, contents );
	
					attr( col.mData.sort, cell );
					attr( col.mData.type, cell );
					attr( col.mData.filter, cell );
				}
				else {
					// Depending on the `data` option for the columns the data can
					// be read to either an object or an array.
					if ( objectRead ) {
						if ( ! col._setter ) {
							// Cache the setter function
							col._setter = _fnSetObjectDataFn( col.mData );
						}
						col._setter( d, contents );
					}
					else {
						d[i] = contents;
					}
				}
			}
	
			i++;
		};
	
		if ( td ) {
			// `tr` element was passed in
			while ( td ) {
				name = td.nodeName.toUpperCase();
	
				if ( name == "TD" || name == "TH" ) {
					cellProcess( td );
					tds.push( td );
				}
	
				td = td.nextSibling;
			}
		}
		else {
			// Existing row object passed in
			tds = row.anCells;
	
			for ( var j=0, jen=tds.length ; j<jen ; j++ ) {
				cellProcess( tds[j] );
			}
		}
	
		// Read the ID from the DOM if present
		var rowNode = row.firstChild ? row : row.nTr;
	
		if ( rowNode ) {
			var id = rowNode.getAttribute( 'id' );
	
			if ( id ) {
				_fnSetObjectDataFn( settings.rowId )( d, id );
			}
		}
	
		return {
			data: d,
			cells: tds
		};
	}
	/**
	 * Create a new TR element (and it's TD children) for a row
	 *  @param {object} oSettings dataTables settings object
	 *  @param {int} iRow Row to consider
	 *  @param {node} [nTrIn] TR element to add to the table - optional. If not given,
	 *    DataTables will create a row automatically
	 *  @param {array} [anTds] Array of TD|TH elements for the row - must be given
	 *    if nTr is.
	 *  @memberof DataTable#oApi
	 */
	function _fnCreateTr ( oSettings, iRow, nTrIn, anTds )
	{
		var
			row = oSettings.aoData[iRow],
			rowData = row._aData,
			cells = [],
			nTr, nTd, oCol,
			i, iLen;
	
		if ( row.nTr === null )
		{
			nTr = nTrIn || document.createElement('tr');
	
			row.nTr = nTr;
			row.anCells = cells;
	
			/* Use a private property on the node to allow reserve mapping from the node
			 * to the aoData array for fast look up
			 */
			nTr._DT_RowIndex = iRow;
	
			/* Special parameters can be given by the data source to be used on the row */
			_fnRowAttributes( oSettings, row );
	
			/* Process each column */
			for ( i=0, iLen=oSettings.aoColumns.length ; i<iLen ; i++ )
			{
				oCol = oSettings.aoColumns[i];
	
				nTd = nTrIn ? anTds[i] : document.createElement( oCol.sCellType );
				nTd._DT_CellIndex = {
					row: iRow,
					column: i
				};
				
				cells.push( nTd );
	
				// Need to create the HTML if new, or if a rendering function is defined
				if ( (!nTrIn || oCol.mRender || oCol.mData !== i) &&
					 (!$.isPlainObject(oCol.mData) || oCol.mData._ !== i+'.display')
				) {
					nTd.innerHTML = _fnGetCellData( oSettings, iRow, i, 'display' );
				}
	
				/* Add user defined class */
				if ( oCol.sClass )
				{
					nTd.className += ' '+oCol.sClass;
				}
	
				// Visibility - add or remove as required
				if ( oCol.bVisible && ! nTrIn )
				{
					nTr.appendChild( nTd );
				}
				else if ( ! oCol.bVisible && nTrIn )
				{
					nTd.parentNode.removeChild( nTd );
				}
	
				if ( oCol.fnCreatedCell )
				{
					oCol.fnCreatedCell.call( oSettings.oInstance,
						nTd, _fnGetCellData( oSettings, iRow, i ), rowData, iRow, i
					);
				}
			}
	
			_fnCallbackFire( oSettings, 'aoRowCreatedCallback', null, [nTr, rowData, iRow] );
		}
	
		// Remove once webkit bug 131819 and Chromium bug 365619 have been resolved
		// and deployed
		row.nTr.setAttribute( 'role', 'row' );
	}
	
	
	/**
	 * Add attributes to a row based on the special `DT_*` parameters in a data
	 * source object.
	 *  @param {object} settings DataTables settings object
	 *  @param {object} DataTables row object for the row to be modified
	 *  @memberof DataTable#oApi
	 */
	function _fnRowAttributes( settings, row )
	{
		var tr = row.nTr;
		var data = row._aData;
	
		if ( tr ) {
			var id = settings.rowIdFn( data );
	
			if ( id ) {
				tr.id = id;
			}
	
			if ( data.DT_RowClass ) {
				// Remove any classes added by DT_RowClass before
				var a = data.DT_RowClass.split(' ');
				row.__rowc = row.__rowc ?
					_unique( row.__rowc.concat( a ) ) :
					a;
	
				$(tr)
					.removeClass( row.__rowc.join(' ') )
					.addClass( data.DT_RowClass );
			}
	
			if ( data.DT_RowAttr ) {
				$(tr).attr( data.DT_RowAttr );
			}
	
			if ( data.DT_RowData ) {
				$(tr).data( data.DT_RowData );
			}
		}
	}
	
	
	/**
	 * Create the HTML header for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnBuildHead( oSettings )
	{
		var i, ien, cell, row, column;
		var thead = oSettings.nTHead;
		var tfoot = oSettings.nTFoot;
		var createHeader = $('th, td', thead).length === 0;
		var classes = oSettings.oClasses;
		var columns = oSettings.aoColumns;
	
		if ( createHeader ) {
			row = $('<tr/>').appendTo( thead );
		}
	
		for ( i=0, ien=columns.length ; i<ien ; i++ ) {
			column = columns[i];
			cell = $( column.nTh ).addClass( column.sClass );
	
			if ( createHeader ) {
				cell.appendTo( row );
			}
	
			// 1.11 move into sorting
			if ( oSettings.oFeatures.bSort ) {
				cell.addClass( column.sSortingClass );
	
				if ( column.bSortable !== false ) {
					cell
						.attr( 'tabindex', oSettings.iTabIndex )
						.attr( 'aria-controls', oSettings.sTableId );
	
					_fnSortAttachListener( oSettings, column.nTh, i );
				}
			}
	
			if ( column.sTitle != cell[0].innerHTML ) {
				cell.html( column.sTitle );
			}
	
			_fnRenderer( oSettings, 'header' )(
				oSettings, cell, column, classes
			);
		}
	
		if ( createHeader ) {
			_fnDetectHeader( oSettings.aoHeader, thead );
		}
		
		/* ARIA role for the rows */
	 	$(thead).find('>tr').attr('role', 'row');
	
		/* Deal with the footer - add classes if required */
		$(thead).find('>tr>th, >tr>td').addClass( classes.sHeaderTH );
		$(tfoot).find('>tr>th, >tr>td').addClass( classes.sFooterTH );
	
		// Cache the footer cells. Note that we only take the cells from the first
		// row in the footer. If there is more than one row the user wants to
		// interact with, they need to use the table().foot() method. Note also this
		// allows cells to be used for multiple columns using colspan
		if ( tfoot !== null ) {
			var cells = oSettings.aoFooter[0];
	
			for ( i=0, ien=cells.length ; i<ien ; i++ ) {
				column = columns[i];
				column.nTf = cells[i].cell;
	
				if ( column.sClass ) {
					$(column.nTf).addClass( column.sClass );
				}
			}
		}
	}
	
	
	/**
	 * Draw the header (or footer) element based on the column visibility states. The
	 * methodology here is to use the layout array from _fnDetectHeader, modified for
	 * the instantaneous column visibility, to construct the new layout. The grid is
	 * traversed over cell at a time in a rows x columns grid fashion, although each
	 * cell insert can cover multiple elements in the grid - which is tracks using the
	 * aApplied array. Cell inserts in the grid will only occur where there isn't
	 * already a cell in that position.
	 *  @param {object} oSettings dataTables settings object
	 *  @param array {objects} aoSource Layout array from _fnDetectHeader
	 *  @param {boolean} [bIncludeHidden=false] If true then include the hidden columns in the calc,
	 *  @memberof DataTable#oApi
	 */
	function _fnDrawHead( oSettings, aoSource, bIncludeHidden )
	{
		var i, iLen, j, jLen, k, kLen, n, nLocalTr;
		var aoLocal = [];
		var aApplied = [];
		var iColumns = oSettings.aoColumns.length;
		var iRowspan, iColspan;
	
		if ( ! aoSource )
		{
			return;
		}
	
		if (  bIncludeHidden === undefined )
		{
			bIncludeHidden = false;
		}
	
		/* Make a copy of the master layout array, but without the visible columns in it */
		for ( i=0, iLen=aoSource.length ; i<iLen ; i++ )
		{
			aoLocal[i] = aoSource[i].slice();
			aoLocal[i].nTr = aoSource[i].nTr;
	
			/* Remove any columns which are currently hidden */
			for ( j=iColumns-1 ; j>=0 ; j-- )
			{
				if ( !oSettings.aoColumns[j].bVisible && !bIncludeHidden )
				{
					aoLocal[i].splice( j, 1 );
				}
			}
	
			/* Prep the applied array - it needs an element for each row */
			aApplied.push( [] );
		}
	
		for ( i=0, iLen=aoLocal.length ; i<iLen ; i++ )
		{
			nLocalTr = aoLocal[i].nTr;
	
			/* All cells are going to be replaced, so empty out the row */
			if ( nLocalTr )
			{
				while( (n = nLocalTr.firstChild) )
				{
					nLocalTr.removeChild( n );
				}
			}
	
			for ( j=0, jLen=aoLocal[i].length ; j<jLen ; j++ )
			{
				iRowspan = 1;
				iColspan = 1;
	
				/* Check to see if there is already a cell (row/colspan) covering our target
				 * insert point. If there is, then there is nothing to do.
				 */
				if ( aApplied[i][j] === undefined )
				{
					nLocalTr.appendChild( aoLocal[i][j].cell );
					aApplied[i][j] = 1;
	
					/* Expand the cell to cover as many rows as needed */
					while ( aoLocal[i+iRowspan] !== undefined &&
					        aoLocal[i][j].cell == aoLocal[i+iRowspan][j].cell )
					{
						aApplied[i+iRowspan][j] = 1;
						iRowspan++;
					}
	
					/* Expand the cell to cover as many columns as needed */
					while ( aoLocal[i][j+iColspan] !== undefined &&
					        aoLocal[i][j].cell == aoLocal[i][j+iColspan].cell )
					{
						/* Must update the applied array over the rows for the columns */
						for ( k=0 ; k<iRowspan ; k++ )
						{
							aApplied[i+k][j+iColspan] = 1;
						}
						iColspan++;
					}
	
					/* Do the actual expansion in the DOM */
					$(aoLocal[i][j].cell)
						.attr('rowspan', iRowspan)
						.attr('colspan', iColspan);
				}
			}
		}
	}
	
	
	/**
	 * Insert the required TR nodes into the table for display
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnDraw( oSettings )
	{
		/* Provide a pre-callback function which can be used to cancel the draw is false is returned */
		var aPreDraw = _fnCallbackFire( oSettings, 'aoPreDrawCallback', 'preDraw', [oSettings] );
		if ( $.inArray( false, aPreDraw ) !== -1 )
		{
			_fnProcessingDisplay( oSettings, false );
			return;
		}
	
		var i, iLen, n;
		var anRows = [];
		var iRowCount = 0;
		var asStripeClasses = oSettings.asStripeClasses;
		var iStripes = asStripeClasses.length;
		var iOpenRows = oSettings.aoOpenRows.length;
		var oLang = oSettings.oLanguage;
		var iInitDisplayStart = oSettings.iInitDisplayStart;
		var bServerSide = _fnDataSource( oSettings ) == 'ssp';
		var aiDisplay = oSettings.aiDisplay;
	
		oSettings.bDrawing = true;
	
		/* Check and see if we have an initial draw position from state saving */
		if ( iInitDisplayStart !== undefined && iInitDisplayStart !== -1 )
		{
			oSettings._iDisplayStart = bServerSide ?
				iInitDisplayStart :
				iInitDisplayStart >= oSettings.fnRecordsDisplay() ?
					0 :
					iInitDisplayStart;
	
			oSettings.iInitDisplayStart = -1;
		}
	
		var iDisplayStart = oSettings._iDisplayStart;
		var iDisplayEnd = oSettings.fnDisplayEnd();
	
		/* Server-side processing draw intercept */
		if ( oSettings.bDeferLoading )
		{
			oSettings.bDeferLoading = false;
			oSettings.iDraw++;
			_fnProcessingDisplay( oSettings, false );
		}
		else if ( !bServerSide )
		{
			oSettings.iDraw++;
		}
		else if ( !oSettings.bDestroying && !_fnAjaxUpdate( oSettings ) )
		{
			return;
		}
	
		if ( aiDisplay.length !== 0 )
		{
			var iStart = bServerSide ? 0 : iDisplayStart;
			var iEnd = bServerSide ? oSettings.aoData.length : iDisplayEnd;
	
			for ( var j=iStart ; j<iEnd ; j++ )
			{
				var iDataIndex = aiDisplay[j];
				var aoData = oSettings.aoData[ iDataIndex ];
				if ( aoData.nTr === null )
				{
					_fnCreateTr( oSettings, iDataIndex );
				}
	
				var nRow = aoData.nTr;
	
				/* Remove the old striping classes and then add the new one */
				if ( iStripes !== 0 )
				{
					var sStripe = asStripeClasses[ iRowCount % iStripes ];
					if ( aoData._sRowStripe != sStripe )
					{
						$(nRow).removeClass( aoData._sRowStripe ).addClass( sStripe );
						aoData._sRowStripe = sStripe;
					}
				}
	
				// Row callback functions - might want to manipulate the row
				// iRowCount and j are not currently documented. Are they at all
				// useful?
				_fnCallbackFire( oSettings, 'aoRowCallback', null,
					[nRow, aoData._aData, iRowCount, j] );
	
				anRows.push( nRow );
				iRowCount++;
			}
		}
		else
		{
			/* Table is empty - create a row with an empty message in it */
			var sZero = oLang.sZeroRecords;
			if ( oSettings.iDraw == 1 &&  _fnDataSource( oSettings ) == 'ajax' )
			{
				sZero = oLang.sLoadingRecords;
			}
			else if ( oLang.sEmptyTable && oSettings.fnRecordsTotal() === 0 )
			{
				sZero = oLang.sEmptyTable;
			}
	
			anRows[ 0 ] = $( '<tr/>', { 'class': iStripes ? asStripeClasses[0] : '' } )
				.append( $('<td />', {
					'valign':  'top',
					'colSpan': _fnVisbleColumns( oSettings ),
					'class':   oSettings.oClasses.sRowEmpty
				} ).html( sZero ) )[0];
		}
	
		/* Header and footer callbacks */
		_fnCallbackFire( oSettings, 'aoHeaderCallback', 'header', [ $(oSettings.nTHead).children('tr')[0],
			_fnGetDataMaster( oSettings ), iDisplayStart, iDisplayEnd, aiDisplay ] );
	
		_fnCallbackFire( oSettings, 'aoFooterCallback', 'footer', [ $(oSettings.nTFoot).children('tr')[0],
			_fnGetDataMaster( oSettings ), iDisplayStart, iDisplayEnd, aiDisplay ] );
	
		var body = $(oSettings.nTBody);
	
		body.children().detach();
		body.append( $(anRows) );
	
		/* Call all required callback functions for the end of a draw */
		_fnCallbackFire( oSettings, 'aoDrawCallback', 'draw', [oSettings] );
	
		/* Draw is complete, sorting and filtering must be as well */
		oSettings.bSorted = false;
		oSettings.bFiltered = false;
		oSettings.bDrawing = false;
	}
	
	
	/**
	 * Redraw the table - taking account of the various features which are enabled
	 *  @param {object} oSettings dataTables settings object
	 *  @param {boolean} [holdPosition] Keep the current paging position. By default
	 *    the paging is reset to the first page
	 *  @memberof DataTable#oApi
	 */
	function _fnReDraw( settings, holdPosition )
	{
		var
			features = settings.oFeatures,
			sort     = features.bSort,
			filter   = features.bFilter;
	
		if ( sort ) {
			_fnSort( settings );
		}
	
		if ( filter ) {
			_fnFilterComplete( settings, settings.oPreviousSearch );
		}
		else {
			// No filtering, so we want to just use the display master
			settings.aiDisplay = settings.aiDisplayMaster.slice();
		}
	
		if ( holdPosition !== true ) {
			settings._iDisplayStart = 0;
		}
	
		// Let any modules know about the draw hold position state (used by
		// scrolling internally)
		settings._drawHold = holdPosition;
	
		_fnDraw( settings );
	
		settings._drawHold = false;
	}
	
	
	/**
	 * Add the options to the page HTML for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnAddOptionsHtml ( oSettings )
	{
		var classes = oSettings.oClasses;
		var table = $(oSettings.nTable);
		var holding = $('<div/>').insertBefore( table ); // Holding element for speed
		var features = oSettings.oFeatures;
	
		// All DataTables are wrapped in a div
		var insert = $('<div/>', {
			id:      oSettings.sTableId+'_wrapper',
			'class': classes.sWrapper + (oSettings.nTFoot ? '' : ' '+classes.sNoFooter)
		} );
	
		oSettings.nHolding = holding[0];
		oSettings.nTableWrapper = insert[0];
		oSettings.nTableReinsertBefore = oSettings.nTable.nextSibling;
	
		/* Loop over the user set positioning and place the elements as needed */
		var aDom = oSettings.sDom.split('');
		var featureNode, cOption, nNewNode, cNext, sAttr, j;
		for ( var i=0 ; i<aDom.length ; i++ )
		{
			featureNode = null;
			cOption = aDom[i];
	
			if ( cOption == '<' )
			{
				/* New container div */
				nNewNode = $('<div/>')[0];
	
				/* Check to see if we should append an id and/or a class name to the container */
				cNext = aDom[i+1];
				if ( cNext == "'" || cNext == '"' )
				{
					sAttr = "";
					j = 2;
					while ( aDom[i+j] != cNext )
					{
						sAttr += aDom[i+j];
						j++;
					}
	
					/* Replace jQuery UI constants @todo depreciated */
					if ( sAttr == "H" )
					{
						sAttr = classes.sJUIHeader;
					}
					else if ( sAttr == "F" )
					{
						sAttr = classes.sJUIFooter;
					}
	
					/* The attribute can be in the format of "#id.class", "#id" or "class" This logic
					 * breaks the string into parts and applies them as needed
					 */
					if ( sAttr.indexOf('.') != -1 )
					{
						var aSplit = sAttr.split('.');
						nNewNode.id = aSplit[0].substr(1, aSplit[0].length-1);
						nNewNode.className = aSplit[1];
					}
					else if ( sAttr.charAt(0) == "#" )
					{
						nNewNode.id = sAttr.substr(1, sAttr.length-1);
					}
					else
					{
						nNewNode.className = sAttr;
					}
	
					i += j; /* Move along the position array */
				}
	
				insert.append( nNewNode );
				insert = $(nNewNode);
			}
			else if ( cOption == '>' )
			{
				/* End container div */
				insert = insert.parent();
			}
			// @todo Move options into their own plugins?
			else if ( cOption == 'l' && features.bPaginate && features.bLengthChange )
			{
				/* Length */
				featureNode = _fnFeatureHtmlLength( oSettings );
			}
			else if ( cOption == 'f' && features.bFilter )
			{
				/* Filter */
				featureNode = _fnFeatureHtmlFilter( oSettings );
			}
			else if ( cOption == 'r' && features.bProcessing )
			{
				/* pRocessing */
				featureNode = _fnFeatureHtmlProcessing( oSettings );
			}
			else if ( cOption == 't' )
			{
				/* Table */
				featureNode = _fnFeatureHtmlTable( oSettings );
			}
			else if ( cOption ==  'i' && features.bInfo )
			{
				/* Info */
				featureNode = _fnFeatureHtmlInfo( oSettings );
			}
			else if ( cOption == 'p' && features.bPaginate )
			{
				/* Pagination */
				featureNode = _fnFeatureHtmlPaginate( oSettings );
			}
			else if ( DataTable.ext.feature.length !== 0 )
			{
				/* Plug-in features */
				var aoFeatures = DataTable.ext.feature;
				for ( var k=0, kLen=aoFeatures.length ; k<kLen ; k++ )
				{
					if ( cOption == aoFeatures[k].cFeature )
					{
						featureNode = aoFeatures[k].fnInit( oSettings );
						break;
					}
				}
			}
	
			/* Add to the 2D features array */
			if ( featureNode )
			{
				var aanFeatures = oSettings.aanFeatures;
	
				if ( ! aanFeatures[cOption] )
				{
					aanFeatures[cOption] = [];
				}
	
				aanFeatures[cOption].push( featureNode );
				insert.append( featureNode );
			}
		}
	
		/* Built our DOM structure - replace the holding div with what we want */
		holding.replaceWith( insert );
		oSettings.nHolding = null;
	}
	
	
	/**
	 * Use the DOM source to create up an array of header cells. The idea here is to
	 * create a layout grid (array) of rows x columns, which contains a reference
	 * to the cell that that point in the grid (regardless of col/rowspan), such that
	 * any column / row could be removed and the new grid constructed
	 *  @param array {object} aLayout Array to store the calculated layout in
	 *  @param {node} nThead The header/footer element for the table
	 *  @memberof DataTable#oApi
	 */
	function _fnDetectHeader ( aLayout, nThead )
	{
		var nTrs = $(nThead).children('tr');
		var nTr, nCell;
		var i, k, l, iLen, jLen, iColShifted, iColumn, iColspan, iRowspan;
		var bUnique;
		var fnShiftCol = function ( a, i, j ) {
			var k = a[i];
	                while ( k[j] ) {
				j++;
			}
			return j;
		};
	
		aLayout.splice( 0, aLayout.length );
	
		/* We know how many rows there are in the layout - so prep it */
		for ( i=0, iLen=nTrs.length ; i<iLen ; i++ )
		{
			aLayout.push( [] );
		}
	
		/* Calculate a layout array */
		for ( i=0, iLen=nTrs.length ; i<iLen ; i++ )
		{
			nTr = nTrs[i];
			iColumn = 0;
	
			/* For every cell in the row... */
			nCell = nTr.firstChild;
			while ( nCell ) {
				if ( nCell.nodeName.toUpperCase() == "TD" ||
				     nCell.nodeName.toUpperCase() == "TH" )
				{
					/* Get the col and rowspan attributes from the DOM and sanitise them */
					iColspan = nCell.getAttribute('colspan') * 1;
					iRowspan = nCell.getAttribute('rowspan') * 1;
					iColspan = (!iColspan || iColspan===0 || iColspan===1) ? 1 : iColspan;
					iRowspan = (!iRowspan || iRowspan===0 || iRowspan===1) ? 1 : iRowspan;
	
					/* There might be colspan cells already in this row, so shift our target
					 * accordingly
					 */
					iColShifted = fnShiftCol( aLayout, i, iColumn );
	
					/* Cache calculation for unique columns */
					bUnique = iColspan === 1 ? true : false;
	
					/* If there is col / rowspan, copy the information into the layout grid */
					for ( l=0 ; l<iColspan ; l++ )
					{
						for ( k=0 ; k<iRowspan ; k++ )
						{
							aLayout[i+k][iColShifted+l] = {
								"cell": nCell,
								"unique": bUnique
							};
							aLayout[i+k].nTr = nTr;
						}
					}
				}
				nCell = nCell.nextSibling;
			}
		}
	}
	
	
	/**
	 * Get an array of unique th elements, one for each column
	 *  @param {object} oSettings dataTables settings object
	 *  @param {node} nHeader automatically detect the layout from this node - optional
	 *  @param {array} aLayout thead/tfoot layout from _fnDetectHeader - optional
	 *  @returns array {node} aReturn list of unique th's
	 *  @memberof DataTable#oApi
	 */
	function _fnGetUniqueThs ( oSettings, nHeader, aLayout )
	{
		var aReturn = [];
		if ( !aLayout )
		{
			aLayout = oSettings.aoHeader;
			if ( nHeader )
			{
				aLayout = [];
				_fnDetectHeader( aLayout, nHeader );
			}
		}
	
		for ( var i=0, iLen=aLayout.length ; i<iLen ; i++ )
		{
			for ( var j=0, jLen=aLayout[i].length ; j<jLen ; j++ )
			{
				if ( aLayout[i][j].unique &&
					 (!aReturn[j] || !oSettings.bSortCellsTop) )
				{
					aReturn[j] = aLayout[i][j].cell;
				}
			}
		}
	
		return aReturn;
	}
	
	/**
	 * Create an Ajax call based on the table's settings, taking into account that
	 * parameters can have multiple forms, and backwards compatibility.
	 *
	 * @param {object} oSettings dataTables settings object
	 * @param {array} data Data to send to the server, required by
	 *     DataTables - may be augmented by developer callbacks
	 * @param {function} fn Callback function to run when data is obtained
	 */
	function _fnBuildAjax( oSettings, data, fn )
	{
		// Compatibility with 1.9-, allow fnServerData and event to manipulate
		_fnCallbackFire( oSettings, 'aoServerParams', 'serverParams', [data] );
	
		// Convert to object based for 1.10+ if using the old array scheme which can
		// come from server-side processing or serverParams
		if ( data && $.isArray(data) ) {
			var tmp = {};
			var rbracket = /(.*?)\[\]$/;
	
			$.each( data, function (key, val) {
				var match = val.name.match(rbracket);
	
				if ( match ) {
					// Support for arrays
					var name = match[0];
	
					if ( ! tmp[ name ] ) {
						tmp[ name ] = [];
					}
					tmp[ name ].push( val.value );
				}
				else {
					tmp[val.name] = val.value;
				}
			} );
			data = tmp;
		}
	
		var ajaxData;
		var ajax = oSettings.ajax;
		var instance = oSettings.oInstance;
		var callback = function ( json ) {
			_fnCallbackFire( oSettings, null, 'xhr', [oSettings, json, oSettings.jqXHR] );
			fn( json );
		};
	
		if ( $.isPlainObject( ajax ) && ajax.data )
		{
			ajaxData = ajax.data;
	
			var newData = $.isFunction( ajaxData ) ?
				ajaxData( data, oSettings ) :  // fn can manipulate data or return
				ajaxData;                      // an object object or array to merge
	
			// If the function returned something, use that alone
			data = $.isFunction( ajaxData ) && newData ?
				newData :
				$.extend( true, data, newData );
	
			// Remove the data property as we've resolved it already and don't want
			// jQuery to do it again (it is restored at the end of the function)
			delete ajax.data;
		}
	
		var baseAjax = {
			"data": data,
			"success": function (json) {
				var error = json.error || json.sError;
				if ( error ) {
					_fnLog( oSettings, 0, error );
				}
	
				oSettings.json = json;
				callback( json );
			},
			"dataType": "json",
			"cache": false,
			"type": oSettings.sServerMethod,
			"error": function (xhr, error, thrown) {
				var ret = _fnCallbackFire( oSettings, null, 'xhr', [oSettings, null, oSettings.jqXHR] );
	
				if ( $.inArray( true, ret ) === -1 ) {
					if ( error == "parsererror" ) {
						_fnLog( oSettings, 0, 'Invalid JSON response', 1 );
					}
					else if ( xhr.readyState === 4 ) {
						_fnLog( oSettings, 0, 'Ajax error', 7 );
					}
				}
	
				_fnProcessingDisplay( oSettings, false );
			}
		};
	
		// Store the data submitted for the API
		oSettings.oAjaxData = data;
	
		// Allow plug-ins and external processes to modify the data
		_fnCallbackFire( oSettings, null, 'preXhr', [oSettings, data] );
	
		if ( oSettings.fnServerData )
		{
			// DataTables 1.9- compatibility
			oSettings.fnServerData.call( instance,
				oSettings.sAjaxSource,
				$.map( data, function (val, key) { // Need to convert back to 1.9 trad format
					return { name: key, value: val };
				} ),
				callback,
				oSettings
			);
		}
		else if ( oSettings.sAjaxSource || typeof ajax === 'string' )
		{
			// DataTables 1.9- compatibility
			oSettings.jqXHR = $.ajax( $.extend( baseAjax, {
				url: ajax || oSettings.sAjaxSource
			} ) );
		}
		else if ( $.isFunction( ajax ) )
		{
			// Is a function - let the caller define what needs to be done
			oSettings.jqXHR = ajax.call( instance, data, callback, oSettings );
		}
		else
		{
			// Object to extend the base settings
			oSettings.jqXHR = $.ajax( $.extend( baseAjax, ajax ) );
	
			// Restore for next time around
			ajax.data = ajaxData;
		}
	}
	
	
	/**
	 * Update the table using an Ajax call
	 *  @param {object} settings dataTables settings object
	 *  @returns {boolean} Block the table drawing or not
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxUpdate( settings )
	{
		if ( settings.bAjaxDataGet ) {
			settings.iDraw++;
			_fnProcessingDisplay( settings, true );
	
			_fnBuildAjax(
				settings,
				_fnAjaxParameters( settings ),
				function(json) {
					_fnAjaxUpdateDraw( settings, json );
				}
			);
	
			return false;
		}
		return true;
	}
	
	
	/**
	 * Build up the parameters in an object needed for a server-side processing
	 * request. Note that this is basically done twice, is different ways - a modern
	 * method which is used by default in DataTables 1.10 which uses objects and
	 * arrays, or the 1.9- method with is name / value pairs. 1.9 method is used if
	 * the sAjaxSource option is used in the initialisation, or the legacyAjax
	 * option is set.
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {bool} block the table drawing or not
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxParameters( settings )
	{
		var
			columns = settings.aoColumns,
			columnCount = columns.length,
			features = settings.oFeatures,
			preSearch = settings.oPreviousSearch,
			preColSearch = settings.aoPreSearchCols,
			i, data = [], dataProp, column, columnSearch,
			sort = _fnSortFlatten( settings ),
			displayStart = settings._iDisplayStart,
			displayLength = features.bPaginate !== false ?
				settings._iDisplayLength :
				-1;
	
		var param = function ( name, value ) {
			data.push( { 'name': name, 'value': value } );
		};
	
		// DataTables 1.9- compatible method
		param( 'sEcho',          settings.iDraw );
		param( 'iColumns',       columnCount );
		param( 'sColumns',       _pluck( columns, 'sName' ).join(',') );
		param( 'iDisplayStart',  displayStart );
		param( 'iDisplayLength', displayLength );
	
		// DataTables 1.10+ method
		var d = {
			draw:    settings.iDraw,
			columns: [],
			order:   [],
			start:   displayStart,
			length:  displayLength,
			search:  {
				value: preSearch.sSearch,
				regex: preSearch.bRegex
			}
		};
	
		for ( i=0 ; i<columnCount ; i++ ) {
			column = columns[i];
			columnSearch = preColSearch[i];
			dataProp = typeof column.mData=="function" ? 'function' : column.mData ;
	
			d.columns.push( {
				data:       dataProp,
				name:       column.sName,
				searchable: column.bSearchable,
				orderable:  column.bSortable,
				search:     {
					value: columnSearch.sSearch,
					regex: columnSearch.bRegex
				}
			} );
	
			param( "mDataProp_"+i, dataProp );
	
			if ( features.bFilter ) {
				param( 'sSearch_'+i,     columnSearch.sSearch );
				param( 'bRegex_'+i,      columnSearch.bRegex );
				param( 'bSearchable_'+i, column.bSearchable );
			}
	
			if ( features.bSort ) {
				param( 'bSortable_'+i, column.bSortable );
			}
		}
	
		if ( features.bFilter ) {
			param( 'sSearch', preSearch.sSearch );
			param( 'bRegex', preSearch.bRegex );
		}
	
		if ( features.bSort ) {
			$.each( sort, function ( i, val ) {
				d.order.push( { column: val.col, dir: val.dir } );
	
				param( 'iSortCol_'+i, val.col );
				param( 'sSortDir_'+i, val.dir );
			} );
	
			param( 'iSortingCols', sort.length );
		}
	
		// If the legacy.ajax parameter is null, then we automatically decide which
		// form to use, based on sAjaxSource
		var legacy = DataTable.ext.legacy.ajax;
		if ( legacy === null ) {
			return settings.sAjaxSource ? data : d;
		}
	
		// Otherwise, if legacy has been specified then we use that to decide on the
		// form
		return legacy ? data : d;
	}
	
	
	/**
	 * Data the data from the server (nuking the old) and redraw the table
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} json json data return from the server.
	 *  @param {string} json.sEcho Tracking flag for DataTables to match requests
	 *  @param {int} json.iTotalRecords Number of records in the data set, not accounting for filtering
	 *  @param {int} json.iTotalDisplayRecords Number of records in the data set, accounting for filtering
	 *  @param {array} json.aaData The data to display on this page
	 *  @param {string} [json.sColumns] Column ordering (sName, comma separated)
	 *  @memberof DataTable#oApi
	 */
	function _fnAjaxUpdateDraw ( settings, json )
	{
		// v1.10 uses camelCase variables, while 1.9 uses Hungarian notation.
		// Support both
		var compat = function ( old, modern ) {
			return json[old] !== undefined ? json[old] : json[modern];
		};
	
		var data = _fnAjaxDataSrc( settings, json );
		var draw            = compat( 'sEcho',                'draw' );
		var recordsTotal    = compat( 'iTotalRecords',        'recordsTotal' );
		var recordsFiltered = compat( 'iTotalDisplayRecords', 'recordsFiltered' );
	
		if ( draw ) {
			// Protect against out of sequence returns
			if ( draw*1 < settings.iDraw ) {
				return;
			}
			settings.iDraw = draw * 1;
		}
	
		_fnClearTable( settings );
		settings._iRecordsTotal   = parseInt(recordsTotal, 10);
		settings._iRecordsDisplay = parseInt(recordsFiltered, 10);
	
		for ( var i=0, ien=data.length ; i<ien ; i++ ) {
			_fnAddData( settings, data[i] );
		}
		settings.aiDisplay = settings.aiDisplayMaster.slice();
	
		settings.bAjaxDataGet = false;
		_fnDraw( settings );
	
		if ( ! settings._bInitComplete ) {
			_fnInitComplete( settings, json );
		}
	
		settings.bAjaxDataGet = true;
		_fnProcessingDisplay( settings, false );
	}
	
	
	/**
	 * Get the data from the JSON data source to use for drawing a table. Using
	 * `_fnGetObjectDataFn` allows the data to be sourced from a property of the
	 * source object, or from a processing function.
	 *  @param {object} oSettings dataTables settings object
	 *  @param  {object} json Data source object / array from the server
	 *  @return {array} Array of data to use
	 */
	function _fnAjaxDataSrc ( oSettings, json )
	{
		var dataSrc = $.isPlainObject( oSettings.ajax ) && oSettings.ajax.dataSrc !== undefined ?
			oSettings.ajax.dataSrc :
			oSettings.sAjaxDataProp; // Compatibility with 1.9-.
	
		// Compatibility with 1.9-. In order to read from aaData, check if the
		// default has been changed, if not, check for aaData
		if ( dataSrc === 'data' ) {
			return json.aaData || json[dataSrc];
		}
	
		return dataSrc !== "" ?
			_fnGetObjectDataFn( dataSrc )( json ) :
			json;
	}
	
	/**
	 * Generate the node required for filtering text
	 *  @returns {node} Filter control element
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlFilter ( settings )
	{
		var classes = settings.oClasses;
		var tableId = settings.sTableId;
		var language = settings.oLanguage;
		var previousSearch = settings.oPreviousSearch;
		var features = settings.aanFeatures;
		var input = '<input type="search" class="'+classes.sFilterInput+'"/>';
	
		var str = language.sSearch;
		str = str.match(/_INPUT_/) ?
			str.replace('_INPUT_', input) :
			str+input;
	
		var filter = $('<div/>', {
				'id': ! features.f ? tableId+'_filter' : null,
				'class': classes.sFilter
			} )
			.append( $('<label/>' ).append( str ) );
	
		var searchFn = function() {
			/* Update all other filter input elements for the new display */
			var n = features.f;
			var val = !this.value ? "" : this.value; // mental IE8 fix :-(
	
			/* Now do the filter */
			if ( val != previousSearch.sSearch ) {
				_fnFilterComplete( settings, {
					"sSearch": val,
					"bRegex": previousSearch.bRegex,
					"bSmart": previousSearch.bSmart ,
					"bCaseInsensitive": previousSearch.bCaseInsensitive
				} );
	
				// Need to redraw, without resorting
				settings._iDisplayStart = 0;
				_fnDraw( settings );
			}
		};
	
		var searchDelay = settings.searchDelay !== null ?
			settings.searchDelay :
			_fnDataSource( settings ) === 'ssp' ?
				400 :
				0;
	
		var jqFilter = $('input', filter)
			.val( previousSearch.sSearch )
			.attr( 'placeholder', language.sSearchPlaceholder )
			.on(
				'keyup.DT search.DT input.DT paste.DT cut.DT',
				searchDelay ?
					_fnThrottle( searchFn, searchDelay ) :
					searchFn
			)
			.on( 'keypress.DT', function(e) {
				/* Prevent form submission */
				if ( e.keyCode == 13 ) {
					return false;
				}
			} )
			.attr('aria-controls', tableId);
	
		// Update the input elements whenever the table is filtered
		$(settings.nTable).on( 'search.dt.DT', function ( ev, s ) {
			if ( settings === s ) {
				// IE9 throws an 'unknown error' if document.activeElement is used
				// inside an iframe or frame...
				try {
					if ( jqFilter[0] !== document.activeElement ) {
						jqFilter.val( previousSearch.sSearch );
					}
				}
				catch ( e ) {}
			}
		} );
	
		return filter[0];
	}
	
	
	/**
	 * Filter the table using both the global filter and column based filtering
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} oSearch search information
	 *  @param {int} [iForce] force a research of the master array (1) or not (undefined or 0)
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterComplete ( oSettings, oInput, iForce )
	{
		var oPrevSearch = oSettings.oPreviousSearch;
		var aoPrevSearch = oSettings.aoPreSearchCols;
		var fnSaveFilter = function ( oFilter ) {
			/* Save the filtering values */
			oPrevSearch.sSearch = oFilter.sSearch;
			oPrevSearch.bRegex = oFilter.bRegex;
			oPrevSearch.bSmart = oFilter.bSmart;
			oPrevSearch.bCaseInsensitive = oFilter.bCaseInsensitive;
		};
		var fnRegex = function ( o ) {
			// Backwards compatibility with the bEscapeRegex option
			return o.bEscapeRegex !== undefined ? !o.bEscapeRegex : o.bRegex;
		};
	
		// Resolve any column types that are unknown due to addition or invalidation
		// @todo As per sort - can this be moved into an event handler?
		_fnColumnTypes( oSettings );
	
		/* In server-side processing all filtering is done by the server, so no point hanging around here */
		if ( _fnDataSource( oSettings ) != 'ssp' )
		{
			/* Global filter */
			_fnFilter( oSettings, oInput.sSearch, iForce, fnRegex(oInput), oInput.bSmart, oInput.bCaseInsensitive );
			fnSaveFilter( oInput );
	
			/* Now do the individual column filter */
			for ( var i=0 ; i<aoPrevSearch.length ; i++ )
			{
				_fnFilterColumn( oSettings, aoPrevSearch[i].sSearch, i, fnRegex(aoPrevSearch[i]),
					aoPrevSearch[i].bSmart, aoPrevSearch[i].bCaseInsensitive );
			}
	
			/* Custom filtering */
			_fnFilterCustom( oSettings );
		}
		else
		{
			fnSaveFilter( oInput );
		}
	
		/* Tell the draw function we have been filtering */
		oSettings.bFiltered = true;
		_fnCallbackFire( oSettings, null, 'search', [oSettings] );
	}
	
	
	/**
	 * Apply custom filtering functions
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterCustom( settings )
	{
		var filters = DataTable.ext.search;
		var displayRows = settings.aiDisplay;
		var row, rowIdx;
	
		for ( var i=0, ien=filters.length ; i<ien ; i++ ) {
			var rows = [];
	
			// Loop over each row and see if it should be included
			for ( var j=0, jen=displayRows.length ; j<jen ; j++ ) {
				rowIdx = displayRows[ j ];
				row = settings.aoData[ rowIdx ];
	
				if ( filters[i]( settings, row._aFilterData, rowIdx, row._aData, j ) ) {
					rows.push( rowIdx );
				}
			}
	
			// So the array reference doesn't break set the results into the
			// existing array
			displayRows.length = 0;
			$.merge( displayRows, rows );
		}
	}
	
	
	/**
	 * Filter the table on a per-column basis
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sInput string to filter on
	 *  @param {int} iColumn column to filter
	 *  @param {bool} bRegex treat search string as a regular expression or not
	 *  @param {bool} bSmart use smart filtering or not
	 *  @param {bool} bCaseInsensitive Do case insenstive matching or not
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterColumn ( settings, searchStr, colIdx, regex, smart, caseInsensitive )
	{
		if ( searchStr === '' ) {
			return;
		}
	
		var data;
		var out = [];
		var display = settings.aiDisplay;
		var rpSearch = _fnFilterCreateSearch( searchStr, regex, smart, caseInsensitive );
	
		for ( var i=0 ; i<display.length ; i++ ) {
			data = settings.aoData[ display[i] ]._aFilterData[ colIdx ];
	
			if ( rpSearch.test( data ) ) {
				out.push( display[i] );
			}
		}
	
		settings.aiDisplay = out;
	}
	
	
	/**
	 * Filter the data table based on user input and draw the table
	 *  @param {object} settings dataTables settings object
	 *  @param {string} input string to filter on
	 *  @param {int} force optional - force a research of the master array (1) or not (undefined or 0)
	 *  @param {bool} regex treat as a regular expression or not
	 *  @param {bool} smart perform smart filtering or not
	 *  @param {bool} caseInsensitive Do case insenstive matching or not
	 *  @memberof DataTable#oApi
	 */
	function _fnFilter( settings, input, force, regex, smart, caseInsensitive )
	{
		var rpSearch = _fnFilterCreateSearch( input, regex, smart, caseInsensitive );
		var prevSearch = settings.oPreviousSearch.sSearch;
		var displayMaster = settings.aiDisplayMaster;
		var display, invalidated, i;
		var filtered = [];
	
		// Need to take account of custom filtering functions - always filter
		if ( DataTable.ext.search.length !== 0 ) {
			force = true;
		}
	
		// Check if any of the rows were invalidated
		invalidated = _fnFilterData( settings );
	
		// If the input is blank - we just want the full data set
		if ( input.length <= 0 ) {
			settings.aiDisplay = displayMaster.slice();
		}
		else {
			// New search - start from the master array
			if ( invalidated ||
				 force ||
				 prevSearch.length > input.length ||
				 input.indexOf(prevSearch) !== 0 ||
				 settings.bSorted // On resort, the display master needs to be
				                  // re-filtered since indexes will have changed
			) {
				settings.aiDisplay = displayMaster.slice();
			}
	
			// Search the display array
			display = settings.aiDisplay;
	
			for ( i=0 ; i<display.length ; i++ ) {
				if ( rpSearch.test( settings.aoData[ display[i] ]._sFilterRow ) ) {
					filtered.push( display[i] );
				}
			}
	
			settings.aiDisplay = filtered;
		}
	}
	
	
	/**
	 * Build a regular expression object suitable for searching a table
	 *  @param {string} sSearch string to search for
	 *  @param {bool} bRegex treat as a regular expression or not
	 *  @param {bool} bSmart perform smart filtering or not
	 *  @param {bool} bCaseInsensitive Do case insensitive matching or not
	 *  @returns {RegExp} constructed object
	 *  @memberof DataTable#oApi
	 */
	function _fnFilterCreateSearch( search, regex, smart, caseInsensitive )
	{
		search = regex ?
			search :
			_fnEscapeRegex( search );
		
		if ( smart ) {
			/* For smart filtering we want to allow the search to work regardless of
			 * word order. We also want double quoted text to be preserved, so word
			 * order is important - a la google. So this is what we want to
			 * generate:
			 * 
			 * ^(?=.*?\bone\b)(?=.*?\btwo three\b)(?=.*?\bfour\b).*$
			 */
			var a = $.map( search.match( /"[^"]+"|[^ ]+/g ) || [''], function ( word ) {
				if ( word.charAt(0) === '"' ) {
					var m = word.match( /^"(.*)"$/ );
					word = m ? m[1] : word;
				}
	
				return word.replace('"', '');
			} );
	
			search = '^(?=.*?'+a.join( ')(?=.*?' )+').*$';
		}
	
		return new RegExp( search, caseInsensitive ? 'i' : '' );
	}
	
	
	/**
	 * Escape a string such that it can be used in a regular expression
	 *  @param {string} sVal string to escape
	 *  @returns {string} escaped string
	 *  @memberof DataTable#oApi
	 */
	var _fnEscapeRegex = DataTable.util.escapeRegex;
	
	var __filter_div = $('<div>')[0];
	var __filter_div_textContent = __filter_div.textContent !== undefined;
	
	// Update the filtering data for each row if needed (by invalidation or first run)
	function _fnFilterData ( settings )
	{
		var columns = settings.aoColumns;
		var column;
		var i, j, ien, jen, filterData, cellData, row;
		var fomatters = DataTable.ext.type.search;
		var wasInvalidated = false;
	
		for ( i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			row = settings.aoData[i];
	
			if ( ! row._aFilterData ) {
				filterData = [];
	
				for ( j=0, jen=columns.length ; j<jen ; j++ ) {
					column = columns[j];
	
					if ( column.bSearchable ) {
						cellData = _fnGetCellData( settings, i, j, 'filter' );
	
						if ( fomatters[ column.sType ] ) {
							cellData = fomatters[ column.sType ]( cellData );
						}
	
						// Search in DataTables 1.10 is string based. In 1.11 this
						// should be altered to also allow strict type checking.
						if ( cellData === null ) {
							cellData = '';
						}
	
						if ( typeof cellData !== 'string' && cellData.toString ) {
							cellData = cellData.toString();
						}
					}
					else {
						cellData = '';
					}
	
					// If it looks like there is an HTML entity in the string,
					// attempt to decode it so sorting works as expected. Note that
					// we could use a single line of jQuery to do this, but the DOM
					// method used here is much faster http://jsperf.com/html-decode
					if ( cellData.indexOf && cellData.indexOf('&') !== -1 ) {
						__filter_div.innerHTML = cellData;
						cellData = __filter_div_textContent ?
							__filter_div.textContent :
							__filter_div.innerText;
					}
	
					if ( cellData.replace ) {
						cellData = cellData.replace(/[\r\n]/g, '');
					}
	
					filterData.push( cellData );
				}
	
				row._aFilterData = filterData;
				row._sFilterRow = filterData.join('  ');
				wasInvalidated = true;
			}
		}
	
		return wasInvalidated;
	}
	
	
	/**
	 * Convert from the internal Hungarian notation to camelCase for external
	 * interaction
	 *  @param {object} obj Object to convert
	 *  @returns {object} Inverted object
	 *  @memberof DataTable#oApi
	 */
	function _fnSearchToCamel ( obj )
	{
		return {
			search:          obj.sSearch,
			smart:           obj.bSmart,
			regex:           obj.bRegex,
			caseInsensitive: obj.bCaseInsensitive
		};
	}
	
	
	
	/**
	 * Convert from camelCase notation to the internal Hungarian. We could use the
	 * Hungarian convert function here, but this is cleaner
	 *  @param {object} obj Object to convert
	 *  @returns {object} Inverted object
	 *  @memberof DataTable#oApi
	 */
	function _fnSearchToHung ( obj )
	{
		return {
			sSearch:          obj.search,
			bSmart:           obj.smart,
			bRegex:           obj.regex,
			bCaseInsensitive: obj.caseInsensitive
		};
	}
	
	/**
	 * Generate the node required for the info display
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {node} Information element
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlInfo ( settings )
	{
		var
			tid = settings.sTableId,
			nodes = settings.aanFeatures.i,
			n = $('<div/>', {
				'class': settings.oClasses.sInfo,
				'id': ! nodes ? tid+'_info' : null
			} );
	
		if ( ! nodes ) {
			// Update display on each draw
			settings.aoDrawCallback.push( {
				"fn": _fnUpdateInfo,
				"sName": "information"
			} );
	
			n
				.attr( 'role', 'status' )
				.attr( 'aria-live', 'polite' );
	
			// Table is described by our info div
			$(settings.nTable).attr( 'aria-describedby', tid+'_info' );
		}
	
		return n[0];
	}
	
	
	/**
	 * Update the information elements in the display
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnUpdateInfo ( settings )
	{
		/* Show information about the table */
		var nodes = settings.aanFeatures.i;
		if ( nodes.length === 0 ) {
			return;
		}
	
		var
			lang  = settings.oLanguage,
			start = settings._iDisplayStart+1,
			end   = settings.fnDisplayEnd(),
			max   = settings.fnRecordsTotal(),
			total = settings.fnRecordsDisplay(),
			out   = total ?
				lang.sInfo :
				lang.sInfoEmpty;
	
		if ( total !== max ) {
			/* Record set after filtering */
			out += ' ' + lang.sInfoFiltered;
		}
	
		// Convert the macros
		out += lang.sInfoPostFix;
		out = _fnInfoMacros( settings, out );
	
		var callback = lang.fnInfoCallback;
		if ( callback !== null ) {
			out = callback.call( settings.oInstance,
				settings, start, end, max, total, out
			);
		}
	
		$(nodes).html( out );
	}
	
	
	function _fnInfoMacros ( settings, str )
	{
		// When infinite scrolling, we are always starting at 1. _iDisplayStart is used only
		// internally
		var
			formatter  = settings.fnFormatNumber,
			start      = settings._iDisplayStart+1,
			len        = settings._iDisplayLength,
			vis        = settings.fnRecordsDisplay(),
			all        = len === -1;
	
		return str.
			replace(/_START_/g, formatter.call( settings, start ) ).
			replace(/_END_/g,   formatter.call( settings, settings.fnDisplayEnd() ) ).
			replace(/_MAX_/g,   formatter.call( settings, settings.fnRecordsTotal() ) ).
			replace(/_TOTAL_/g, formatter.call( settings, vis ) ).
			replace(/_PAGE_/g,  formatter.call( settings, all ? 1 : Math.ceil( start / len ) ) ).
			replace(/_PAGES_/g, formatter.call( settings, all ? 1 : Math.ceil( vis / len ) ) );
	}
	
	
	
	/**
	 * Draw the table for the first time, adding all required features
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnInitialise ( settings )
	{
		var i, iLen, iAjaxStart=settings.iInitDisplayStart;
		var columns = settings.aoColumns, column;
		var features = settings.oFeatures;
		var deferLoading = settings.bDeferLoading; // value modified by the draw
	
		/* Ensure that the table data is fully initialised */
		if ( ! settings.bInitialised ) {
			setTimeout( function(){ _fnInitialise( settings ); }, 200 );
			return;
		}
	
		/* Show the display HTML options */
		_fnAddOptionsHtml( settings );
	
		/* Build and draw the header / footer for the table */
		_fnBuildHead( settings );
		_fnDrawHead( settings, settings.aoHeader );
		_fnDrawHead( settings, settings.aoFooter );
	
		/* Okay to show that something is going on now */
		_fnProcessingDisplay( settings, true );
	
		/* Calculate sizes for columns */
		if ( features.bAutoWidth ) {
			_fnCalculateColumnWidths( settings );
		}
	
		for ( i=0, iLen=columns.length ; i<iLen ; i++ ) {
			column = columns[i];
	
			if ( column.sWidth ) {
				column.nTh.style.width = _fnStringToCss( column.sWidth );
			}
		}
	
		_fnCallbackFire( settings, null, 'preInit', [settings] );
	
		// If there is default sorting required - let's do it. The sort function
		// will do the drawing for us. Otherwise we draw the table regardless of the
		// Ajax source - this allows the table to look initialised for Ajax sourcing
		// data (show 'loading' message possibly)
		_fnReDraw( settings );
	
		// Server-side processing init complete is done by _fnAjaxUpdateDraw
		var dataSrc = _fnDataSource( settings );
		if ( dataSrc != 'ssp' || deferLoading ) {
			// if there is an ajax source load the data
			if ( dataSrc == 'ajax' ) {
				_fnBuildAjax( settings, [], function(json) {
					var aData = _fnAjaxDataSrc( settings, json );
	
					// Got the data - add it to the table
					for ( i=0 ; i<aData.length ; i++ ) {
						_fnAddData( settings, aData[i] );
					}
	
					// Reset the init display for cookie saving. We've already done
					// a filter, and therefore cleared it before. So we need to make
					// it appear 'fresh'
					settings.iInitDisplayStart = iAjaxStart;
	
					_fnReDraw( settings );
	
					_fnProcessingDisplay( settings, false );
					_fnInitComplete( settings, json );
				}, settings );
			}
			else {
				_fnProcessingDisplay( settings, false );
				_fnInitComplete( settings );
			}
		}
	}
	
	
	/**
	 * Draw the table for the first time, adding all required features
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} [json] JSON from the server that completed the table, if using Ajax source
	 *    with client-side processing (optional)
	 *  @memberof DataTable#oApi
	 */
	function _fnInitComplete ( settings, json )
	{
		settings._bInitComplete = true;
	
		// When data was added after the initialisation (data or Ajax) we need to
		// calculate the column sizing
		if ( json || settings.oInit.aaData ) {
			_fnAdjustColumnSizing( settings );
		}
	
		_fnCallbackFire( settings, null, 'plugin-init', [settings, json] );
		_fnCallbackFire( settings, 'aoInitComplete', 'init', [settings, json] );
	}
	
	
	function _fnLengthChange ( settings, val )
	{
		var len = parseInt( val, 10 );
		settings._iDisplayLength = len;
	
		_fnLengthOverflow( settings );
	
		// Fire length change event
		_fnCallbackFire( settings, null, 'length', [settings, len] );
	}
	
	
	/**
	 * Generate the node required for user display length changing
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Display length feature node
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlLength ( settings )
	{
		var
			classes  = settings.oClasses,
			tableId  = settings.sTableId,
			menu     = settings.aLengthMenu,
			d2       = $.isArray( menu[0] ),
			lengths  = d2 ? menu[0] : menu,
			language = d2 ? menu[1] : menu;
	
		var select = $('<select/>', {
			'name':          tableId+'_length',
			'aria-controls': tableId,
			'class':         classes.sLengthSelect
		} );
	
		for ( var i=0, ien=lengths.length ; i<ien ; i++ ) {
			select[0][ i ] = new Option(
				typeof language[i] === 'number' ?
					settings.fnFormatNumber( language[i] ) :
					language[i],
				lengths[i]
			);
		}
	
		var div = $('<div><label/></div>').addClass( classes.sLength );
		if ( ! settings.aanFeatures.l ) {
			div[0].id = tableId+'_length';
		}
	
		div.children().append(
			settings.oLanguage.sLengthMenu.replace( '_MENU_', select[0].outerHTML )
		);
	
		// Can't use `select` variable as user might provide their own and the
		// reference is broken by the use of outerHTML
		$('select', div)
			.val( settings._iDisplayLength )
			.on( 'change.DT', function(e) {
				_fnLengthChange( settings, $(this).val() );
				_fnDraw( settings );
			} );
	
		// Update node value whenever anything changes the table's length
		$(settings.nTable).on( 'length.dt.DT', function (e, s, len) {
			if ( settings === s ) {
				$('select', div).val( len );
			}
		} );
	
		return div[0];
	}
	
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Note that most of the paging logic is done in
	 * DataTable.ext.pager
	 */
	
	/**
	 * Generate the node required for default pagination
	 *  @param {object} oSettings dataTables settings object
	 *  @returns {node} Pagination feature node
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlPaginate ( settings )
	{
		var
			type   = settings.sPaginationType,
			plugin = DataTable.ext.pager[ type ],
			modern = typeof plugin === 'function',
			redraw = function( settings ) {
				_fnDraw( settings );
			},
			node = $('<div/>').addClass( settings.oClasses.sPaging + type )[0],
			features = settings.aanFeatures;
	
		if ( ! modern ) {
			plugin.fnInit( settings, node, redraw );
		}
	
		/* Add a draw callback for the pagination on first instance, to update the paging display */
		if ( ! features.p )
		{
			node.id = settings.sTableId+'_paginate';
	
			settings.aoDrawCallback.push( {
				"fn": function( settings ) {
					if ( modern ) {
						var
							start      = settings._iDisplayStart,
							len        = settings._iDisplayLength,
							visRecords = settings.fnRecordsDisplay(),
							all        = len === -1,
							page = all ? 0 : Math.ceil( start / len ),
							pages = all ? 1 : Math.ceil( visRecords / len ),
							buttons = plugin(page, pages),
							i, ien;
	
						for ( i=0, ien=features.p.length ; i<ien ; i++ ) {
							_fnRenderer( settings, 'pageButton' )(
								settings, features.p[i], i, buttons, page, pages
							);
						}
					}
					else {
						plugin.fnUpdate( settings, redraw );
					}
				},
				"sName": "pagination"
			} );
		}
	
		return node;
	}
	
	
	/**
	 * Alter the display settings to change the page
	 *  @param {object} settings DataTables settings object
	 *  @param {string|int} action Paging action to take: "first", "previous",
	 *    "next" or "last" or page number to jump to (integer)
	 *  @param [bool] redraw Automatically draw the update or not
	 *  @returns {bool} true page has changed, false - no change
	 *  @memberof DataTable#oApi
	 */
	function _fnPageChange ( settings, action, redraw )
	{
		var
			start     = settings._iDisplayStart,
			len       = settings._iDisplayLength,
			records   = settings.fnRecordsDisplay();
	
		if ( records === 0 || len === -1 )
		{
			start = 0;
		}
		else if ( typeof action === "number" )
		{
			start = action * len;
	
			if ( start > records )
			{
				start = 0;
			}
		}
		else if ( action == "first" )
		{
			start = 0;
		}
		else if ( action == "previous" )
		{
			start = len >= 0 ?
				start - len :
				0;
	
			if ( start < 0 )
			{
			  start = 0;
			}
		}
		else if ( action == "next" )
		{
			if ( start + len < records )
			{
				start += len;
			}
		}
		else if ( action == "last" )
		{
			start = Math.floor( (records-1) / len) * len;
		}
		else
		{
			_fnLog( settings, 0, "Unknown paging action: "+action, 5 );
		}
	
		var changed = settings._iDisplayStart !== start;
		settings._iDisplayStart = start;
	
		if ( changed ) {
			_fnCallbackFire( settings, null, 'page', [settings] );
	
			if ( redraw ) {
				_fnDraw( settings );
			}
		}
	
		return changed;
	}
	
	
	
	/**
	 * Generate the node required for the processing node
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Processing element
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlProcessing ( settings )
	{
		return $('<div/>', {
				'id': ! settings.aanFeatures.r ? settings.sTableId+'_processing' : null,
				'class': settings.oClasses.sProcessing
			} )
			.html( settings.oLanguage.sProcessing )
			.insertBefore( settings.nTable )[0];
	}
	
	
	/**
	 * Display or hide the processing indicator
	 *  @param {object} settings dataTables settings object
	 *  @param {bool} show Show the processing indicator (true) or not (false)
	 *  @memberof DataTable#oApi
	 */
	function _fnProcessingDisplay ( settings, show )
	{
		if ( settings.oFeatures.bProcessing ) {
			$(settings.aanFeatures.r).css( 'display', show ? 'block' : 'none' );
		}
	
		_fnCallbackFire( settings, null, 'processing', [settings, show] );
	}
	
	/**
	 * Add any control elements for the table - specifically scrolling
	 *  @param {object} settings dataTables settings object
	 *  @returns {node} Node to add to the DOM
	 *  @memberof DataTable#oApi
	 */
	function _fnFeatureHtmlTable ( settings )
	{
		var table = $(settings.nTable);
	
		// Add the ARIA grid role to the table
		table.attr( 'role', 'grid' );
	
		// Scrolling from here on in
		var scroll = settings.oScroll;
	
		if ( scroll.sX === '' && scroll.sY === '' ) {
			return settings.nTable;
		}
	
		var scrollX = scroll.sX;
		var scrollY = scroll.sY;
		var classes = settings.oClasses;
		var caption = table.children('caption');
		var captionSide = caption.length ? caption[0]._captionSide : null;
		var headerClone = $( table[0].cloneNode(false) );
		var footerClone = $( table[0].cloneNode(false) );
		var footer = table.children('tfoot');
		var _div = '<div/>';
		var size = function ( s ) {
			return !s ? null : _fnStringToCss( s );
		};
	
		if ( ! footer.length ) {
			footer = null;
		}
	
		/*
		 * The HTML structure that we want to generate in this function is:
		 *  div - scroller
		 *    div - scroll head
		 *      div - scroll head inner
		 *        table - scroll head table
		 *          thead - thead
		 *    div - scroll body
		 *      table - table (master table)
		 *        thead - thead clone for sizing
		 *        tbody - tbody
		 *    div - scroll foot
		 *      div - scroll foot inner
		 *        table - scroll foot table
		 *          tfoot - tfoot
		 */
		var scroller = $( _div, { 'class': classes.sScrollWrapper } )
			.append(
				$(_div, { 'class': classes.sScrollHead } )
					.css( {
						overflow: 'hidden',
						position: 'relative',
						border: 0,
						width: scrollX ? size(scrollX) : '100%'
					} )
					.append(
						$(_div, { 'class': classes.sScrollHeadInner } )
							.css( {
								'box-sizing': 'content-box',
								width: scroll.sXInner || '100%'
							} )
							.append(
								headerClone
									.removeAttr('id')
									.css( 'margin-left', 0 )
									.append( captionSide === 'top' ? caption : null )
									.append(
										table.children('thead')
									)
							)
					)
			)
			.append(
				$(_div, { 'class': classes.sScrollBody } )
					.css( {
						position: 'relative',
						overflow: 'auto',
						width: size( scrollX )
					} )
					.append( table )
			);
	
		if ( footer ) {
			scroller.append(
				$(_div, { 'class': classes.sScrollFoot } )
					.css( {
						overflow: 'hidden',
						border: 0,
						width: scrollX ? size(scrollX) : '100%'
					} )
					.append(
						$(_div, { 'class': classes.sScrollFootInner } )
							.append(
								footerClone
									.removeAttr('id')
									.css( 'margin-left', 0 )
									.append( captionSide === 'bottom' ? caption : null )
									.append(
										table.children('tfoot')
									)
							)
					)
			);
		}
	
		var children = scroller.children();
		var scrollHead = children[0];
		var scrollBody = children[1];
		var scrollFoot = footer ? children[2] : null;
	
		// When the body is scrolled, then we also want to scroll the headers
		if ( scrollX ) {
			$(scrollBody).on( 'scroll.DT', function (e) {
				var scrollLeft = this.scrollLeft;
	
				scrollHead.scrollLeft = scrollLeft;
	
				if ( footer ) {
					scrollFoot.scrollLeft = scrollLeft;
				}
			} );
		}
	
		$(scrollBody).css(
			scrollY && scroll.bCollapse ? 'max-height' : 'height', 
			scrollY
		);
	
		settings.nScrollHead = scrollHead;
		settings.nScrollBody = scrollBody;
		settings.nScrollFoot = scrollFoot;
	
		// On redraw - align columns
		settings.aoDrawCallback.push( {
			"fn": _fnScrollDraw,
			"sName": "scrolling"
		} );
	
		return scroller[0];
	}
	
	
	
	/**
	 * Update the header, footer and body tables for resizing - i.e. column
	 * alignment.
	 *
	 * Welcome to the most horrible function DataTables. The process that this
	 * function follows is basically:
	 *   1. Re-create the table inside the scrolling div
	 *   2. Take live measurements from the DOM
	 *   3. Apply the measurements to align the columns
	 *   4. Clean up
	 *
	 *  @param {object} settings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnScrollDraw ( settings )
	{
		// Given that this is such a monster function, a lot of variables are use
		// to try and keep the minimised size as small as possible
		var
			scroll         = settings.oScroll,
			scrollX        = scroll.sX,
			scrollXInner   = scroll.sXInner,
			scrollY        = scroll.sY,
			barWidth       = scroll.iBarWidth,
			divHeader      = $(settings.nScrollHead),
			divHeaderStyle = divHeader[0].style,
			divHeaderInner = divHeader.children('div'),
			divHeaderInnerStyle = divHeaderInner[0].style,
			divHeaderTable = divHeaderInner.children('table'),
			divBodyEl      = settings.nScrollBody,
			divBody        = $(divBodyEl),
			divBodyStyle   = divBodyEl.style,
			divFooter      = $(settings.nScrollFoot),
			divFooterInner = divFooter.children('div'),
			divFooterTable = divFooterInner.children('table'),
			header         = $(settings.nTHead),
			table          = $(settings.nTable),
			tableEl        = table[0],
			tableStyle     = tableEl.style,
			footer         = settings.nTFoot ? $(settings.nTFoot) : null,
			browser        = settings.oBrowser,
			ie67           = browser.bScrollOversize,
			dtHeaderCells  = _pluck( settings.aoColumns, 'nTh' ),
			headerTrgEls, footerTrgEls,
			headerSrcEls, footerSrcEls,
			headerCopy, footerCopy,
			headerWidths=[], footerWidths=[],
			headerContent=[], footerContent=[],
			idx, correction, sanityWidth,
			zeroOut = function(nSizer) {
				var style = nSizer.style;
				style.paddingTop = "0";
				style.paddingBottom = "0";
				style.borderTopWidth = "0";
				style.borderBottomWidth = "0";
				style.height = 0;
			};
	
		// If the scrollbar visibility has changed from the last draw, we need to
		// adjust the column sizes as the table width will have changed to account
		// for the scrollbar
		var scrollBarVis = divBodyEl.scrollHeight > divBodyEl.clientHeight;
		
		if ( settings.scrollBarVis !== scrollBarVis && settings.scrollBarVis !== undefined ) {
			settings.scrollBarVis = scrollBarVis;
			_fnAdjustColumnSizing( settings );
			return; // adjust column sizing will call this function again
		}
		else {
			settings.scrollBarVis = scrollBarVis;
		}
	
		/*
		 * 1. Re-create the table inside the scrolling div
		 */
	
		// Remove the old minimised thead and tfoot elements in the inner table
		table.children('thead, tfoot').remove();
	
		if ( footer ) {
			footerCopy = footer.clone().prependTo( table );
			footerTrgEls = footer.find('tr'); // the original tfoot is in its own table and must be sized
			footerSrcEls = footerCopy.find('tr');
		}
	
		// Clone the current header and footer elements and then place it into the inner table
		headerCopy = header.clone().prependTo( table );
		headerTrgEls = header.find('tr'); // original header is in its own table
		headerSrcEls = headerCopy.find('tr');
		headerCopy.find('th, td').removeAttr('tabindex');
	
	
		/*
		 * 2. Take live measurements from the DOM - do not alter the DOM itself!
		 */
	
		// Remove old sizing and apply the calculated column widths
		// Get the unique column headers in the newly created (cloned) header. We want to apply the
		// calculated sizes to this header
		if ( ! scrollX )
		{
			divBodyStyle.width = '100%';
			divHeader[0].style.width = '100%';
		}
	
		$.each( _fnGetUniqueThs( settings, headerCopy ), function ( i, el ) {
			idx = _fnVisibleToColumnIndex( settings, i );
			el.style.width = settings.aoColumns[idx].sWidth;
		} );
	
		if ( footer ) {
			_fnApplyToChildren( function(n) {
				n.style.width = "";
			}, footerSrcEls );
		}
	
		// Size the table as a whole
		sanityWidth = table.outerWidth();
		if ( scrollX === "" ) {
			// No x scrolling
			tableStyle.width = "100%";
	
			// IE7 will make the width of the table when 100% include the scrollbar
			// - which is shouldn't. When there is a scrollbar we need to take this
			// into account.
			if ( ie67 && (table.find('tbody').height() > divBodyEl.offsetHeight ||
				divBody.css('overflow-y') == "scroll")
			) {
				tableStyle.width = _fnStringToCss( table.outerWidth() - barWidth);
			}
	
			// Recalculate the sanity width
			sanityWidth = table.outerWidth();
		}
		else if ( scrollXInner !== "" ) {
			// legacy x scroll inner has been given - use it
			tableStyle.width = _fnStringToCss(scrollXInner);
	
			// Recalculate the sanity width
			sanityWidth = table.outerWidth();
		}
	
		// Hidden header should have zero height, so remove padding and borders. Then
		// set the width based on the real headers
	
		// Apply all styles in one pass
		_fnApplyToChildren( zeroOut, headerSrcEls );
	
		// Read all widths in next pass
		_fnApplyToChildren( function(nSizer) {
			headerContent.push( nSizer.innerHTML );
			headerWidths.push( _fnStringToCss( $(nSizer).css('width') ) );
		}, headerSrcEls );
	
		// Apply all widths in final pass
		_fnApplyToChildren( function(nToSize, i) {
			// Only apply widths to the DataTables detected header cells - this
			// prevents complex headers from having contradictory sizes applied
			if ( $.inArray( nToSize, dtHeaderCells ) !== -1 ) {
				nToSize.style.width = headerWidths[i];
			}
		}, headerTrgEls );
	
		$(headerSrcEls).height(0);
	
		/* Same again with the footer if we have one */
		if ( footer )
		{
			_fnApplyToChildren( zeroOut, footerSrcEls );
	
			_fnApplyToChildren( function(nSizer) {
				footerContent.push( nSizer.innerHTML );
				footerWidths.push( _fnStringToCss( $(nSizer).css('width') ) );
			}, footerSrcEls );
	
			_fnApplyToChildren( function(nToSize, i) {
				nToSize.style.width = footerWidths[i];
			}, footerTrgEls );
	
			$(footerSrcEls).height(0);
		}
	
	
		/*
		 * 3. Apply the measurements
		 */
	
		// "Hide" the header and footer that we used for the sizing. We need to keep
		// the content of the cell so that the width applied to the header and body
		// both match, but we want to hide it completely. We want to also fix their
		// width to what they currently are
		_fnApplyToChildren( function(nSizer, i) {
			nSizer.innerHTML = '<div class="dataTables_sizing" style="height:0;overflow:hidden;">'+headerContent[i]+'</div>';
			nSizer.style.width = headerWidths[i];
		}, headerSrcEls );
	
		if ( footer )
		{
			_fnApplyToChildren( function(nSizer, i) {
				nSizer.innerHTML = '<div class="dataTables_sizing" style="height:0;overflow:hidden;">'+footerContent[i]+'</div>';
				nSizer.style.width = footerWidths[i];
			}, footerSrcEls );
		}
	
		// Sanity check that the table is of a sensible width. If not then we are going to get
		// misalignment - try to prevent this by not allowing the table to shrink below its min width
		if ( table.outerWidth() < sanityWidth )
		{
			// The min width depends upon if we have a vertical scrollbar visible or not */
			correction = ((divBodyEl.scrollHeight > divBodyEl.offsetHeight ||
				divBody.css('overflow-y') == "scroll")) ?
					sanityWidth+barWidth :
					sanityWidth;
	
			// IE6/7 are a law unto themselves...
			if ( ie67 && (divBodyEl.scrollHeight >
				divBodyEl.offsetHeight || divBody.css('overflow-y') == "scroll")
			) {
				tableStyle.width = _fnStringToCss( correction-barWidth );
			}
	
			// And give the user a warning that we've stopped the table getting too small
			if ( scrollX === "" || scrollXInner !== "" ) {
				_fnLog( settings, 1, 'Possible column misalignment', 6 );
			}
		}
		else
		{
			correction = '100%';
		}
	
		// Apply to the container elements
		divBodyStyle.width = _fnStringToCss( correction );
		divHeaderStyle.width = _fnStringToCss( correction );
	
		if ( footer ) {
			settings.nScrollFoot.style.width = _fnStringToCss( correction );
		}
	
	
		/*
		 * 4. Clean up
		 */
		if ( ! scrollY ) {
			/* IE7< puts a vertical scrollbar in place (when it shouldn't be) due to subtracting
			 * the scrollbar height from the visible display, rather than adding it on. We need to
			 * set the height in order to sort this. Don't want to do it in any other browsers.
			 */
			if ( ie67 ) {
				divBodyStyle.height = _fnStringToCss( tableEl.offsetHeight+barWidth );
			}
		}
	
		/* Finally set the width's of the header and footer tables */
		var iOuterWidth = table.outerWidth();
		divHeaderTable[0].style.width = _fnStringToCss( iOuterWidth );
		divHeaderInnerStyle.width = _fnStringToCss( iOuterWidth );
	
		// Figure out if there are scrollbar present - if so then we need a the header and footer to
		// provide a bit more space to allow "overflow" scrolling (i.e. past the scrollbar)
		var bScrolling = table.height() > divBodyEl.clientHeight || divBody.css('overflow-y') == "scroll";
		var padding = 'padding' + (browser.bScrollbarLeft ? 'Left' : 'Right' );
		divHeaderInnerStyle[ padding ] = bScrolling ? barWidth+"px" : "0px";
	
		if ( footer ) {
			divFooterTable[0].style.width = _fnStringToCss( iOuterWidth );
			divFooterInner[0].style.width = _fnStringToCss( iOuterWidth );
			divFooterInner[0].style[padding] = bScrolling ? barWidth+"px" : "0px";
		}
	
		// Correct DOM ordering for colgroup - comes before the thead
		table.children('colgroup').insertBefore( table.children('thead') );
	
		/* Adjust the position of the header in case we loose the y-scrollbar */
		divBody.scroll();
	
		// If sorting or filtering has occurred, jump the scrolling back to the top
		// only if we aren't holding the position
		if ( (settings.bSorted || settings.bFiltered) && ! settings._drawHold ) {
			divBodyEl.scrollTop = 0;
		}
	}
	
	
	
	/**
	 * Apply a given function to the display child nodes of an element array (typically
	 * TD children of TR rows
	 *  @param {function} fn Method to apply to the objects
	 *  @param array {nodes} an1 List of elements to look through for display children
	 *  @param array {nodes} an2 Another list (identical structure to the first) - optional
	 *  @memberof DataTable#oApi
	 */
	function _fnApplyToChildren( fn, an1, an2 )
	{
		var index=0, i=0, iLen=an1.length;
		var nNode1, nNode2;
	
		while ( i < iLen ) {
			nNode1 = an1[i].firstChild;
			nNode2 = an2 ? an2[i].firstChild : null;
	
			while ( nNode1 ) {
				if ( nNode1.nodeType === 1 ) {
					if ( an2 ) {
						fn( nNode1, nNode2, index );
					}
					else {
						fn( nNode1, index );
					}
	
					index++;
				}
	
				nNode1 = nNode1.nextSibling;
				nNode2 = an2 ? nNode2.nextSibling : null;
			}
	
			i++;
		}
	}
	
	
	
	var __re_html_remove = /<.*?>/g;
	
	
	/**
	 * Calculate the width of columns for the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnCalculateColumnWidths ( oSettings )
	{
		var
			table = oSettings.nTable,
			columns = oSettings.aoColumns,
			scroll = oSettings.oScroll,
			scrollY = scroll.sY,
			scrollX = scroll.sX,
			scrollXInner = scroll.sXInner,
			columnCount = columns.length,
			visibleColumns = _fnGetColumns( oSettings, 'bVisible' ),
			headerCells = $('th', oSettings.nTHead),
			tableWidthAttr = table.getAttribute('width'), // from DOM element
			tableContainer = table.parentNode,
			userInputs = false,
			i, column, columnIdx, width, outerWidth,
			browser = oSettings.oBrowser,
			ie67 = browser.bScrollOversize;
	
		var styleWidth = table.style.width;
		if ( styleWidth && styleWidth.indexOf('%') !== -1 ) {
			tableWidthAttr = styleWidth;
		}
	
		/* Convert any user input sizes into pixel sizes */
		for ( i=0 ; i<visibleColumns.length ; i++ ) {
			column = columns[ visibleColumns[i] ];
	
			if ( column.sWidth !== null ) {
				column.sWidth = _fnConvertToWidth( column.sWidthOrig, tableContainer );
	
				userInputs = true;
			}
		}
	
		/* If the number of columns in the DOM equals the number that we have to
		 * process in DataTables, then we can use the offsets that are created by
		 * the web- browser. No custom sizes can be set in order for this to happen,
		 * nor scrolling used
		 */
		if ( ie67 || ! userInputs && ! scrollX && ! scrollY &&
		     columnCount == _fnVisbleColumns( oSettings ) &&
		     columnCount == headerCells.length
		) {
			for ( i=0 ; i<columnCount ; i++ ) {
				var colIdx = _fnVisibleToColumnIndex( oSettings, i );
	
				if ( colIdx !== null ) {
					columns[ colIdx ].sWidth = _fnStringToCss( headerCells.eq(i).width() );
				}
			}
		}
		else
		{
			// Otherwise construct a single row, worst case, table with the widest
			// node in the data, assign any user defined widths, then insert it into
			// the DOM and allow the browser to do all the hard work of calculating
			// table widths
			var tmpTable = $(table).clone() // don't use cloneNode - IE8 will remove events on the main table
				.css( 'visibility', 'hidden' )
				.removeAttr( 'id' );
	
			// Clean up the table body
			tmpTable.find('tbody tr').remove();
			var tr = $('<tr/>').appendTo( tmpTable.find('tbody') );
	
			// Clone the table header and footer - we can't use the header / footer
			// from the cloned table, since if scrolling is active, the table's
			// real header and footer are contained in different table tags
			tmpTable.find('thead, tfoot').remove();
			tmpTable
				.append( $(oSettings.nTHead).clone() )
				.append( $(oSettings.nTFoot).clone() );
	
			// Remove any assigned widths from the footer (from scrolling)
			tmpTable.find('tfoot th, tfoot td').css('width', '');
	
			// Apply custom sizing to the cloned header
			headerCells = _fnGetUniqueThs( oSettings, tmpTable.find('thead')[0] );
	
			for ( i=0 ; i<visibleColumns.length ; i++ ) {
				column = columns[ visibleColumns[i] ];
	
				headerCells[i].style.width = column.sWidthOrig !== null && column.sWidthOrig !== '' ?
					_fnStringToCss( column.sWidthOrig ) :
					'';
	
				// For scrollX we need to force the column width otherwise the
				// browser will collapse it. If this width is smaller than the
				// width the column requires, then it will have no effect
				if ( column.sWidthOrig && scrollX ) {
					$( headerCells[i] ).append( $('<div/>').css( {
						width: column.sWidthOrig,
						margin: 0,
						padding: 0,
						border: 0,
						height: 1
					} ) );
				}
			}
	
			// Find the widest cell for each column and put it into the table
			if ( oSettings.aoData.length ) {
				for ( i=0 ; i<visibleColumns.length ; i++ ) {
					columnIdx = visibleColumns[i];
					column = columns[ columnIdx ];
	
					$( _fnGetWidestNode( oSettings, columnIdx ) )
						.clone( false )
						.append( column.sContentPadding )
						.appendTo( tr );
				}
			}
	
			// Tidy the temporary table - remove name attributes so there aren't
			// duplicated in the dom (radio elements for example)
			$('[name]', tmpTable).removeAttr('name');
	
			// Table has been built, attach to the document so we can work with it.
			// A holding element is used, positioned at the top of the container
			// with minimal height, so it has no effect on if the container scrolls
			// or not. Otherwise it might trigger scrolling when it actually isn't
			// needed
			var holder = $('<div/>').css( scrollX || scrollY ?
					{
						position: 'absolute',
						top: 0,
						left: 0,
						height: 1,
						right: 0,
						overflow: 'hidden'
					} :
					{}
				)
				.append( tmpTable )
				.appendTo( tableContainer );
	
			// When scrolling (X or Y) we want to set the width of the table as 
			// appropriate. However, when not scrolling leave the table width as it
			// is. This results in slightly different, but I think correct behaviour
			if ( scrollX && scrollXInner ) {
				tmpTable.width( scrollXInner );
			}
			else if ( scrollX ) {
				tmpTable.css( 'width', 'auto' );
				tmpTable.removeAttr('width');
	
				// If there is no width attribute or style, then allow the table to
				// collapse
				if ( tmpTable.width() < tableContainer.clientWidth && tableWidthAttr ) {
					tmpTable.width( tableContainer.clientWidth );
				}
			}
			else if ( scrollY ) {
				tmpTable.width( tableContainer.clientWidth );
			}
			else if ( tableWidthAttr ) {
				tmpTable.width( tableWidthAttr );
			}
	
			// Get the width of each column in the constructed table - we need to
			// know the inner width (so it can be assigned to the other table's
			// cells) and the outer width so we can calculate the full width of the
			// table. This is safe since DataTables requires a unique cell for each
			// column, but if ever a header can span multiple columns, this will
			// need to be modified.
			var total = 0;
			for ( i=0 ; i<visibleColumns.length ; i++ ) {
				var cell = $(headerCells[i]);
				var border = cell.outerWidth() - cell.width();
	
				// Use getBounding... where possible (not IE8-) because it can give
				// sub-pixel accuracy, which we then want to round up!
				var bounding = browser.bBounding ?
					Math.ceil( headerCells[i].getBoundingClientRect().width ) :
					cell.outerWidth();
	
				// Total is tracked to remove any sub-pixel errors as the outerWidth
				// of the table might not equal the total given here (IE!).
				total += bounding;
	
				// Width for each column to use
				columns[ visibleColumns[i] ].sWidth = _fnStringToCss( bounding - border );
			}
	
			table.style.width = _fnStringToCss( total );
	
			// Finished with the table - ditch it
			holder.remove();
		}
	
		// If there is a width attr, we want to attach an event listener which
		// allows the table sizing to automatically adjust when the window is
		// resized. Use the width attr rather than CSS, since we can't know if the
		// CSS is a relative value or absolute - DOM read is always px.
		if ( tableWidthAttr ) {
			table.style.width = _fnStringToCss( tableWidthAttr );
		}
	
		if ( (tableWidthAttr || scrollX) && ! oSettings._reszEvt ) {
			var bindResize = function () {
				$(window).on('resize.DT-'+oSettings.sInstance, _fnThrottle( function () {
					_fnAdjustColumnSizing( oSettings );
				} ) );
			};
	
			// IE6/7 will crash if we bind a resize event handler on page load.
			// To be removed in 1.11 which drops IE6/7 support
			if ( ie67 ) {
				setTimeout( bindResize, 1000 );
			}
			else {
				bindResize();
			}
	
			oSettings._reszEvt = true;
		}
	}
	
	
	/**
	 * Throttle the calls to a function. Arguments and context are maintained for
	 * the throttled function
	 *  @param {function} fn Function to be called
	 *  @param {int} [freq=200] call frequency in mS
	 *  @returns {function} wrapped function
	 *  @memberof DataTable#oApi
	 */
	var _fnThrottle = DataTable.util.throttle;
	
	
	/**
	 * Convert a CSS unit width to pixels (e.g. 2em)
	 *  @param {string} width width to be converted
	 *  @param {node} parent parent to get the with for (required for relative widths) - optional
	 *  @returns {int} width in pixels
	 *  @memberof DataTable#oApi
	 */
	function _fnConvertToWidth ( width, parent )
	{
		if ( ! width ) {
			return 0;
		}
	
		var n = $('<div/>')
			.css( 'width', _fnStringToCss( width ) )
			.appendTo( parent || document.body );
	
		var val = n[0].offsetWidth;
		n.remove();
	
		return val;
	}
	
	
	/**
	 * Get the widest node
	 *  @param {object} settings dataTables settings object
	 *  @param {int} colIdx column of interest
	 *  @returns {node} widest table node
	 *  @memberof DataTable#oApi
	 */
	function _fnGetWidestNode( settings, colIdx )
	{
		var idx = _fnGetMaxLenString( settings, colIdx );
		if ( idx < 0 ) {
			return null;
		}
	
		var data = settings.aoData[ idx ];
		return ! data.nTr ? // Might not have been created when deferred rendering
			$('<td/>').html( _fnGetCellData( settings, idx, colIdx, 'display' ) )[0] :
			data.anCells[ colIdx ];
	}
	
	
	/**
	 * Get the maximum strlen for each data column
	 *  @param {object} settings dataTables settings object
	 *  @param {int} colIdx column of interest
	 *  @returns {string} max string length for each column
	 *  @memberof DataTable#oApi
	 */
	function _fnGetMaxLenString( settings, colIdx )
	{
		var s, max=-1, maxIdx = -1;
	
		for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			s = _fnGetCellData( settings, i, colIdx, 'display' )+'';
			s = s.replace( __re_html_remove, '' );
			s = s.replace( /&nbsp;/g, ' ' );
	
			if ( s.length > max ) {
				max = s.length;
				maxIdx = i;
			}
		}
	
		return maxIdx;
	}
	
	
	/**
	 * Append a CSS unit (only if required) to a string
	 *  @param {string} value to css-ify
	 *  @returns {string} value with css unit
	 *  @memberof DataTable#oApi
	 */
	function _fnStringToCss( s )
	{
		if ( s === null ) {
			return '0px';
		}
	
		if ( typeof s == 'number' ) {
			return s < 0 ?
				'0px' :
				s+'px';
		}
	
		// Check it has a unit character already
		return s.match(/\d$/) ?
			s+'px' :
			s;
	}
	
	
	
	function _fnSortFlatten ( settings )
	{
		var
			i, iLen, k, kLen,
			aSort = [],
			aiOrig = [],
			aoColumns = settings.aoColumns,
			aDataSort, iCol, sType, srcCol,
			fixed = settings.aaSortingFixed,
			fixedObj = $.isPlainObject( fixed ),
			nestedSort = [],
			add = function ( a ) {
				if ( a.length && ! $.isArray( a[0] ) ) {
					// 1D array
					nestedSort.push( a );
				}
				else {
					// 2D array
					$.merge( nestedSort, a );
				}
			};
	
		// Build the sort array, with pre-fix and post-fix options if they have been
		// specified
		if ( $.isArray( fixed ) ) {
			add( fixed );
		}
	
		if ( fixedObj && fixed.pre ) {
			add( fixed.pre );
		}
	
		add( settings.aaSorting );
	
		if (fixedObj && fixed.post ) {
			add( fixed.post );
		}
	
		for ( i=0 ; i<nestedSort.length ; i++ )
		{
			srcCol = nestedSort[i][0];
			aDataSort = aoColumns[ srcCol ].aDataSort;
	
			for ( k=0, kLen=aDataSort.length ; k<kLen ; k++ )
			{
				iCol = aDataSort[k];
				sType = aoColumns[ iCol ].sType || 'string';
	
				if ( nestedSort[i]._idx === undefined ) {
					nestedSort[i]._idx = $.inArray( nestedSort[i][1], aoColumns[iCol].asSorting );
				}
	
				aSort.push( {
					src:       srcCol,
					col:       iCol,
					dir:       nestedSort[i][1],
					index:     nestedSort[i]._idx,
					type:      sType,
					formatter: DataTable.ext.type.order[ sType+"-pre" ]
				} );
			}
		}
	
		return aSort;
	}
	
	/**
	 * Change the order of the table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 *  @todo This really needs split up!
	 */
	function _fnSort ( oSettings )
	{
		var
			i, ien, iLen, j, jLen, k, kLen,
			sDataType, nTh,
			aiOrig = [],
			oExtSort = DataTable.ext.type.order,
			aoData = oSettings.aoData,
			aoColumns = oSettings.aoColumns,
			aDataSort, data, iCol, sType, oSort,
			formatters = 0,
			sortCol,
			displayMaster = oSettings.aiDisplayMaster,
			aSort;
	
		// Resolve any column types that are unknown due to addition or invalidation
		// @todo Can this be moved into a 'data-ready' handler which is called when
		//   data is going to be used in the table?
		_fnColumnTypes( oSettings );
	
		aSort = _fnSortFlatten( oSettings );
	
		for ( i=0, ien=aSort.length ; i<ien ; i++ ) {
			sortCol = aSort[i];
	
			// Track if we can use the fast sort algorithm
			if ( sortCol.formatter ) {
				formatters++;
			}
	
			// Load the data needed for the sort, for each cell
			_fnSortData( oSettings, sortCol.col );
		}
	
		/* No sorting required if server-side or no sorting array */
		if ( _fnDataSource( oSettings ) != 'ssp' && aSort.length !== 0 )
		{
			// Create a value - key array of the current row positions such that we can use their
			// current position during the sort, if values match, in order to perform stable sorting
			for ( i=0, iLen=displayMaster.length ; i<iLen ; i++ ) {
				aiOrig[ displayMaster[i] ] = i;
			}
	
			/* Do the sort - here we want multi-column sorting based on a given data source (column)
			 * and sorting function (from oSort) in a certain direction. It's reasonably complex to
			 * follow on it's own, but this is what we want (example two column sorting):
			 *  fnLocalSorting = function(a,b){
			 *    var iTest;
			 *    iTest = oSort['string-asc']('data11', 'data12');
			 *      if (iTest !== 0)
			 *        return iTest;
			 *    iTest = oSort['numeric-desc']('data21', 'data22');
			 *    if (iTest !== 0)
			 *      return iTest;
			 *    return oSort['numeric-asc']( aiOrig[a], aiOrig[b] );
			 *  }
			 * Basically we have a test for each sorting column, if the data in that column is equal,
			 * test the next column. If all columns match, then we use a numeric sort on the row
			 * positions in the original data array to provide a stable sort.
			 *
			 * Note - I know it seems excessive to have two sorting methods, but the first is around
			 * 15% faster, so the second is only maintained for backwards compatibility with sorting
			 * methods which do not have a pre-sort formatting function.
			 */
			if ( formatters === aSort.length ) {
				// All sort types have formatting functions
				displayMaster.sort( function ( a, b ) {
					var
						x, y, k, test, sort,
						len=aSort.length,
						dataA = aoData[a]._aSortData,
						dataB = aoData[b]._aSortData;
	
					for ( k=0 ; k<len ; k++ ) {
						sort = aSort[k];
	
						x = dataA[ sort.col ];
						y = dataB[ sort.col ];
	
						test = x<y ? -1 : x>y ? 1 : 0;
						if ( test !== 0 ) {
							return sort.dir === 'asc' ? test : -test;
						}
					}
	
					x = aiOrig[a];
					y = aiOrig[b];
					return x<y ? -1 : x>y ? 1 : 0;
				} );
			}
			else {
				// Depreciated - remove in 1.11 (providing a plug-in option)
				// Not all sort types have formatting methods, so we have to call their sorting
				// methods.
				displayMaster.sort( function ( a, b ) {
					var
						x, y, k, l, test, sort, fn,
						len=aSort.length,
						dataA = aoData[a]._aSortData,
						dataB = aoData[b]._aSortData;
	
					for ( k=0 ; k<len ; k++ ) {
						sort = aSort[k];
	
						x = dataA[ sort.col ];
						y = dataB[ sort.col ];
	
						fn = oExtSort[ sort.type+"-"+sort.dir ] || oExtSort[ "string-"+sort.dir ];
						test = fn( x, y );
						if ( test !== 0 ) {
							return test;
						}
					}
	
					x = aiOrig[a];
					y = aiOrig[b];
					return x<y ? -1 : x>y ? 1 : 0;
				} );
			}
		}
	
		/* Tell the draw function that we have sorted the data */
		oSettings.bSorted = true;
	}
	
	
	function _fnSortAria ( settings )
	{
		var label;
		var nextSort;
		var columns = settings.aoColumns;
		var aSort = _fnSortFlatten( settings );
		var oAria = settings.oLanguage.oAria;
	
		// ARIA attributes - need to loop all columns, to update all (removing old
		// attributes as needed)
		for ( var i=0, iLen=columns.length ; i<iLen ; i++ )
		{
			var col = columns[i];
			var asSorting = col.asSorting;
			var sTitle = col.sTitle.replace( /<.*?>/g, "" );
			var th = col.nTh;
	
			// IE7 is throwing an error when setting these properties with jQuery's
			// attr() and removeAttr() methods...
			th.removeAttribute('aria-sort');
	
			/* In ARIA only the first sorting column can be marked as sorting - no multi-sort option */
			if ( col.bSortable ) {
				if ( aSort.length > 0 && aSort[0].col == i ) {
					th.setAttribute('aria-sort', aSort[0].dir=="asc" ? "ascending" : "descending" );
					nextSort = asSorting[ aSort[0].index+1 ] || asSorting[0];
				}
				else {
					nextSort = asSorting[0];
				}
	
				label = sTitle + ( nextSort === "asc" ?
					oAria.sSortAscending :
					oAria.sSortDescending
				);
			}
			else {
				label = sTitle;
			}
	
			th.setAttribute('aria-label', label);
		}
	}
	
	
	/**
	 * Function to run on user sort request
	 *  @param {object} settings dataTables settings object
	 *  @param {node} attachTo node to attach the handler to
	 *  @param {int} colIdx column sorting index
	 *  @param {boolean} [append=false] Append the requested sort to the existing
	 *    sort if true (i.e. multi-column sort)
	 *  @param {function} [callback] callback function
	 *  @memberof DataTable#oApi
	 */
	function _fnSortListener ( settings, colIdx, append, callback )
	{
		var col = settings.aoColumns[ colIdx ];
		var sorting = settings.aaSorting;
		var asSorting = col.asSorting;
		var nextSortIdx;
		var next = function ( a, overflow ) {
			var idx = a._idx;
			if ( idx === undefined ) {
				idx = $.inArray( a[1], asSorting );
			}
	
			return idx+1 < asSorting.length ?
				idx+1 :
				overflow ?
					null :
					0;
		};
	
		// Convert to 2D array if needed
		if ( typeof sorting[0] === 'number' ) {
			sorting = settings.aaSorting = [ sorting ];
		}
	
		// If appending the sort then we are multi-column sorting
		if ( append && settings.oFeatures.bSortMulti ) {
			// Are we already doing some kind of sort on this column?
			var sortIdx = $.inArray( colIdx, _pluck(sorting, '0') );
	
			if ( sortIdx !== -1 ) {
				// Yes, modify the sort
				nextSortIdx = next( sorting[sortIdx], true );
	
				if ( nextSortIdx === null && sorting.length === 1 ) {
					nextSortIdx = 0; // can't remove sorting completely
				}
	
				if ( nextSortIdx === null ) {
					sorting.splice( sortIdx, 1 );
				}
				else {
					sorting[sortIdx][1] = asSorting[ nextSortIdx ];
					sorting[sortIdx]._idx = nextSortIdx;
				}
			}
			else {
				// No sort on this column yet
				sorting.push( [ colIdx, asSorting[0], 0 ] );
				sorting[sorting.length-1]._idx = 0;
			}
		}
		else if ( sorting.length && sorting[0][0] == colIdx ) {
			// Single column - already sorting on this column, modify the sort
			nextSortIdx = next( sorting[0] );
	
			sorting.length = 1;
			sorting[0][1] = asSorting[ nextSortIdx ];
			sorting[0]._idx = nextSortIdx;
		}
		else {
			// Single column - sort only on this column
			sorting.length = 0;
			sorting.push( [ colIdx, asSorting[0] ] );
			sorting[0]._idx = 0;
		}
	
		// Run the sort by calling a full redraw
		_fnReDraw( settings );
	
		// callback used for async user interaction
		if ( typeof callback == 'function' ) {
			callback( settings );
		}
	}
	
	
	/**
	 * Attach a sort handler (click) to a node
	 *  @param {object} settings dataTables settings object
	 *  @param {node} attachTo node to attach the handler to
	 *  @param {int} colIdx column sorting index
	 *  @param {function} [callback] callback function
	 *  @memberof DataTable#oApi
	 */
	function _fnSortAttachListener ( settings, attachTo, colIdx, callback )
	{
		var col = settings.aoColumns[ colIdx ];
	
		_fnBindAction( attachTo, {}, function (e) {
			/* If the column is not sortable - don't to anything */
			if ( col.bSortable === false ) {
				return;
			}
	
			// If processing is enabled use a timeout to allow the processing
			// display to be shown - otherwise to it synchronously
			if ( settings.oFeatures.bProcessing ) {
				_fnProcessingDisplay( settings, true );
	
				setTimeout( function() {
					_fnSortListener( settings, colIdx, e.shiftKey, callback );
	
					// In server-side processing, the draw callback will remove the
					// processing display
					if ( _fnDataSource( settings ) !== 'ssp' ) {
						_fnProcessingDisplay( settings, false );
					}
				}, 0 );
			}
			else {
				_fnSortListener( settings, colIdx, e.shiftKey, callback );
			}
		} );
	}
	
	
	/**
	 * Set the sorting classes on table's body, Note: it is safe to call this function
	 * when bSort and bSortClasses are false
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnSortingClasses( settings )
	{
		var oldSort = settings.aLastSort;
		var sortClass = settings.oClasses.sSortColumn;
		var sort = _fnSortFlatten( settings );
		var features = settings.oFeatures;
		var i, ien, colIdx;
	
		if ( features.bSort && features.bSortClasses ) {
			// Remove old sorting classes
			for ( i=0, ien=oldSort.length ; i<ien ; i++ ) {
				colIdx = oldSort[i].src;
	
				// Remove column sorting
				$( _pluck( settings.aoData, 'anCells', colIdx ) )
					.removeClass( sortClass + (i<2 ? i+1 : 3) );
			}
	
			// Add new column sorting
			for ( i=0, ien=sort.length ; i<ien ; i++ ) {
				colIdx = sort[i].src;
	
				$( _pluck( settings.aoData, 'anCells', colIdx ) )
					.addClass( sortClass + (i<2 ? i+1 : 3) );
			}
		}
	
		settings.aLastSort = sort;
	}
	
	
	// Get the data to sort a column, be it from cache, fresh (populating the
	// cache), or from a sort formatter
	function _fnSortData( settings, idx )
	{
		// Custom sorting function - provided by the sort data type
		var column = settings.aoColumns[ idx ];
		var customSort = DataTable.ext.order[ column.sSortDataType ];
		var customData;
	
		if ( customSort ) {
			customData = customSort.call( settings.oInstance, settings, idx,
				_fnColumnIndexToVisible( settings, idx )
			);
		}
	
		// Use / populate cache
		var row, cellData;
		var formatter = DataTable.ext.type.order[ column.sType+"-pre" ];
	
		for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
			row = settings.aoData[i];
	
			if ( ! row._aSortData ) {
				row._aSortData = [];
			}
	
			if ( ! row._aSortData[idx] || customSort ) {
				cellData = customSort ?
					customData[i] : // If there was a custom sort function, use data from there
					_fnGetCellData( settings, i, idx, 'sort' );
	
				row._aSortData[ idx ] = formatter ?
					formatter( cellData ) :
					cellData;
			}
		}
	}
	
	
	
	/**
	 * Save the state of a table
	 *  @param {object} oSettings dataTables settings object
	 *  @memberof DataTable#oApi
	 */
	function _fnSaveState ( settings )
	{
		if ( !settings.oFeatures.bStateSave || settings.bDestroying )
		{
			return;
		}
	
		/* Store the interesting variables */
		var state = {
			time:    +new Date(),
			start:   settings._iDisplayStart,
			length:  settings._iDisplayLength,
			order:   $.extend( true, [], settings.aaSorting ),
			search:  _fnSearchToCamel( settings.oPreviousSearch ),
			columns: $.map( settings.aoColumns, function ( col, i ) {
				return {
					visible: col.bVisible,
					search: _fnSearchToCamel( settings.aoPreSearchCols[i] )
				};
			} )
		};
	
		_fnCallbackFire( settings, "aoStateSaveParams", 'stateSaveParams', [settings, state] );
	
		settings.oSavedState = state;
		settings.fnStateSaveCallback.call( settings.oInstance, settings, state );
	}
	
	
	/**
	 * Attempt to load a saved table state
	 *  @param {object} oSettings dataTables settings object
	 *  @param {object} oInit DataTables init object so we can override settings
	 *  @param {function} callback Callback to execute when the state has been loaded
	 *  @memberof DataTable#oApi
	 */
	function _fnLoadState ( settings, oInit, callback )
	{
		var i, ien;
		var columns = settings.aoColumns;
		var loaded = function ( s ) {
			if ( ! s || ! s.time ) {
				callback();
				return;
			}
	
			// Allow custom and plug-in manipulation functions to alter the saved data set and
			// cancelling of loading by returning false
			var abStateLoad = _fnCallbackFire( settings, 'aoStateLoadParams', 'stateLoadParams', [settings, s] );
			if ( $.inArray( false, abStateLoad ) !== -1 ) {
				callback();
				return;
			}
	
			// Reject old data
			var duration = settings.iStateDuration;
			if ( duration > 0 && s.time < +new Date() - (duration*1000) ) {
				callback();
				return;
			}
	
			// Number of columns have changed - all bets are off, no restore of settings
			if ( s.columns && columns.length !== s.columns.length ) {
				callback();
				return;
			}
	
			// Store the saved state so it might be accessed at any time
			settings.oLoadedState = $.extend( true, {}, s );
	
			// Restore key features - todo - for 1.11 this needs to be done by
			// subscribed events
			if ( s.start !== undefined ) {
				settings._iDisplayStart    = s.start;
				settings.iInitDisplayStart = s.start;
			}
			if ( s.length !== undefined ) {
				settings._iDisplayLength   = s.length;
			}
	
			// Order
			if ( s.order !== undefined ) {
				settings.aaSorting = [];
				$.each( s.order, function ( i, col ) {
					settings.aaSorting.push( col[0] >= columns.length ?
						[ 0, col[1] ] :
						col
					);
				} );
			}
	
			// Search
			if ( s.search !== undefined ) {
				$.extend( settings.oPreviousSearch, _fnSearchToHung( s.search ) );
			}
	
			// Columns
			//
			if ( s.columns ) {
				for ( i=0, ien=s.columns.length ; i<ien ; i++ ) {
					var col = s.columns[i];
	
					// Visibility
					if ( col.visible !== undefined ) {
						columns[i].bVisible = col.visible;
					}
	
					// Search
					if ( col.search !== undefined ) {
						$.extend( settings.aoPreSearchCols[i], _fnSearchToHung( col.search ) );
					}
				}
			}
	
			_fnCallbackFire( settings, 'aoStateLoaded', 'stateLoaded', [settings, s] );
			callback();
		}
	
		if ( ! settings.oFeatures.bStateSave ) {
			callback();
			return;
		}
	
		var state = settings.fnStateLoadCallback.call( settings.oInstance, settings, loaded );
	
		if ( state !== undefined ) {
			loaded( state );
		}
		// otherwise, wait for the loaded callback to be executed
	}
	
	
	/**
	 * Return the settings object for a particular table
	 *  @param {node} table table we are using as a dataTable
	 *  @returns {object} Settings object - or null if not found
	 *  @memberof DataTable#oApi
	 */
	function _fnSettingsFromNode ( table )
	{
		var settings = DataTable.settings;
		var idx = $.inArray( table, _pluck( settings, 'nTable' ) );
	
		return idx !== -1 ?
			settings[ idx ] :
			null;
	}
	
	
	/**
	 * Log an error message
	 *  @param {object} settings dataTables settings object
	 *  @param {int} level log error messages, or display them to the user
	 *  @param {string} msg error message
	 *  @param {int} tn Technical note id to get more information about the error.
	 *  @memberof DataTable#oApi
	 */
	function _fnLog( settings, level, msg, tn )
	{
		msg = 'DataTables warning: '+
			(settings ? 'table id='+settings.sTableId+' - ' : '')+msg;
	
		if ( tn ) {
			msg += '. For more information about this error, please see '+
			'http://datatables.net/tn/'+tn;
		}
	
		if ( ! level  ) {
			// Backwards compatibility pre 1.10
			var ext = DataTable.ext;
			var type = ext.sErrMode || ext.errMode;
	
			if ( settings ) {
				_fnCallbackFire( settings, null, 'error', [ settings, tn, msg ] );
			}
	
			if ( type == 'alert' ) {
				alert( msg );
			}
			else if ( type == 'throw' ) {
				throw new Error(msg);
			}
			else if ( typeof type == 'function' ) {
				type( settings, tn, msg );
			}
		}
		else if ( window.console && console.log ) {
			console.log( msg );
		}
	}
	
	
	/**
	 * See if a property is defined on one object, if so assign it to the other object
	 *  @param {object} ret target object
	 *  @param {object} src source object
	 *  @param {string} name property
	 *  @param {string} [mappedName] name to map too - optional, name used if not given
	 *  @memberof DataTable#oApi
	 */
	function _fnMap( ret, src, name, mappedName )
	{
		if ( $.isArray( name ) ) {
			$.each( name, function (i, val) {
				if ( $.isArray( val ) ) {
					_fnMap( ret, src, val[0], val[1] );
				}
				else {
					_fnMap( ret, src, val );
				}
			} );
	
			return;
		}
	
		if ( mappedName === undefined ) {
			mappedName = name;
		}
	
		if ( src[name] !== undefined ) {
			ret[mappedName] = src[name];
		}
	}
	
	
	/**
	 * Extend objects - very similar to jQuery.extend, but deep copy objects, and
	 * shallow copy arrays. The reason we need to do this, is that we don't want to
	 * deep copy array init values (such as aaSorting) since the dev wouldn't be
	 * able to override them, but we do want to deep copy arrays.
	 *  @param {object} out Object to extend
	 *  @param {object} extender Object from which the properties will be applied to
	 *      out
	 *  @param {boolean} breakRefs If true, then arrays will be sliced to take an
	 *      independent copy with the exception of the `data` or `aaData` parameters
	 *      if they are present. This is so you can pass in a collection to
	 *      DataTables and have that used as your data source without breaking the
	 *      references
	 *  @returns {object} out Reference, just for convenience - out === the return.
	 *  @memberof DataTable#oApi
	 *  @todo This doesn't take account of arrays inside the deep copied objects.
	 */
	function _fnExtend( out, extender, breakRefs )
	{
		var val;
	
		for ( var prop in extender ) {
			if ( extender.hasOwnProperty(prop) ) {
				val = extender[prop];
	
				if ( $.isPlainObject( val ) ) {
					if ( ! $.isPlainObject( out[prop] ) ) {
						out[prop] = {};
					}
					$.extend( true, out[prop], val );
				}
				else if ( breakRefs && prop !== 'data' && prop !== 'aaData' && $.isArray(val) ) {
					out[prop] = val.slice();
				}
				else {
					out[prop] = val;
				}
			}
		}
	
		return out;
	}
	
	
	/**
	 * Bind an event handers to allow a click or return key to activate the callback.
	 * This is good for accessibility since a return on the keyboard will have the
	 * same effect as a click, if the element has focus.
	 *  @param {element} n Element to bind the action to
	 *  @param {object} oData Data object to pass to the triggered function
	 *  @param {function} fn Callback function for when the event is triggered
	 *  @memberof DataTable#oApi
	 */
	function _fnBindAction( n, oData, fn )
	{
		$(n)
			.on( 'click.DT', oData, function (e) {
					n.blur(); // Remove focus outline for mouse users
					fn(e);
				} )
			.on( 'keypress.DT', oData, function (e){
					if ( e.which === 13 ) {
						e.preventDefault();
						fn(e);
					}
				} )
			.on( 'selectstart.DT', function () {
					/* Take the brutal approach to cancelling text selection */
					return false;
				} );
	}
	
	
	/**
	 * Register a callback function. Easily allows a callback function to be added to
	 * an array store of callback functions that can then all be called together.
	 *  @param {object} oSettings dataTables settings object
	 *  @param {string} sStore Name of the array storage for the callbacks in oSettings
	 *  @param {function} fn Function to be called back
	 *  @param {string} sName Identifying name for the callback (i.e. a label)
	 *  @memberof DataTable#oApi
	 */
	function _fnCallbackReg( oSettings, sStore, fn, sName )
	{
		if ( fn )
		{
			oSettings[sStore].push( {
				"fn": fn,
				"sName": sName
			} );
		}
	}
	
	
	/**
	 * Fire callback functions and trigger events. Note that the loop over the
	 * callback array store is done backwards! Further note that you do not want to
	 * fire off triggers in time sensitive applications (for example cell creation)
	 * as its slow.
	 *  @param {object} settings dataTables settings object
	 *  @param {string} callbackArr Name of the array storage for the callbacks in
	 *      oSettings
	 *  @param {string} eventName Name of the jQuery custom event to trigger. If
	 *      null no trigger is fired
	 *  @param {array} args Array of arguments to pass to the callback function /
	 *      trigger
	 *  @memberof DataTable#oApi
	 */
	function _fnCallbackFire( settings, callbackArr, eventName, args )
	{
		var ret = [];
	
		if ( callbackArr ) {
			ret = $.map( settings[callbackArr].slice().reverse(), function (val, i) {
				return val.fn.apply( settings.oInstance, args );
			} );
		}
	
		if ( eventName !== null ) {
			var e = $.Event( eventName+'.dt' );
	
			$(settings.nTable).trigger( e, args );
	
			ret.push( e.result );
		}
	
		return ret;
	}
	
	
	function _fnLengthOverflow ( settings )
	{
		var
			start = settings._iDisplayStart,
			end = settings.fnDisplayEnd(),
			len = settings._iDisplayLength;
	
		/* If we have space to show extra rows (backing up from the end point - then do so */
		if ( start >= end )
		{
			start = end - len;
		}
	
		// Keep the start record on the current page
		start -= (start % len);
	
		if ( len === -1 || start < 0 )
		{
			start = 0;
		}
	
		settings._iDisplayStart = start;
	}
	
	
	function _fnRenderer( settings, type )
	{
		var renderer = settings.renderer;
		var host = DataTable.ext.renderer[type];
	
		if ( $.isPlainObject( renderer ) && renderer[type] ) {
			// Specific renderer for this type. If available use it, otherwise use
			// the default.
			return host[renderer[type]] || host._;
		}
		else if ( typeof renderer === 'string' ) {
			// Common renderer - if there is one available for this type use it,
			// otherwise use the default
			return host[renderer] || host._;
		}
	
		// Use the default
		return host._;
	}
	
	
	/**
	 * Detect the data source being used for the table. Used to simplify the code
	 * a little (ajax) and to make it compress a little smaller.
	 *
	 *  @param {object} settings dataTables settings object
	 *  @returns {string} Data source
	 *  @memberof DataTable#oApi
	 */
	function _fnDataSource ( settings )
	{
		if ( settings.oFeatures.bServerSide ) {
			return 'ssp';
		}
		else if ( settings.ajax || settings.sAjaxSource ) {
			return 'ajax';
		}
		return 'dom';
	}
	

	
	
	/**
	 * Computed structure of the DataTables API, defined by the options passed to
	 * `DataTable.Api.register()` when building the API.
	 *
	 * The structure is built in order to speed creation and extension of the Api
	 * objects since the extensions are effectively pre-parsed.
	 *
	 * The array is an array of objects with the following structure, where this
	 * base array represents the Api prototype base:
	 *
	 *     [
	 *       {
	 *         name:      'data'                -- string   - Property name
	 *         val:       function () {},       -- function - Api method (or undefined if just an object
	 *         methodExt: [ ... ],              -- array    - Array of Api object definitions to extend the method result
	 *         propExt:   [ ... ]               -- array    - Array of Api object definitions to extend the property
	 *       },
	 *       {
	 *         name:     'row'
	 *         val:       {},
	 *         methodExt: [ ... ],
	 *         propExt:   [
	 *           {
	 *             name:      'data'
	 *             val:       function () {},
	 *             methodExt: [ ... ],
	 *             propExt:   [ ... ]
	 *           },
	 *           ...
	 *         ]
	 *       }
	 *     ]
	 *
	 * @type {Array}
	 * @ignore
	 */
	var __apiStruct = [];
	
	
	/**
	 * `Array.prototype` reference.
	 *
	 * @type object
	 * @ignore
	 */
	var __arrayProto = Array.prototype;
	
	
	/**
	 * Abstraction for `context` parameter of the `Api` constructor to allow it to
	 * take several different forms for ease of use.
	 *
	 * Each of the input parameter types will be converted to a DataTables settings
	 * object where possible.
	 *
	 * @param  {string|node|jQuery|object} mixed DataTable identifier. Can be one
	 *   of:
	 *
	 *   * `string` - jQuery selector. Any DataTables' matching the given selector
	 *     with be found and used.
	 *   * `node` - `TABLE` node which has already been formed into a DataTable.
	 *   * `jQuery` - A jQuery object of `TABLE` nodes.
	 *   * `object` - DataTables settings object
	 *   * `DataTables.Api` - API instance
	 * @return {array|null} Matching DataTables settings objects. `null` or
	 *   `undefined` is returned if no matching DataTable is found.
	 * @ignore
	 */
	var _toSettings = function ( mixed )
	{
		var idx, jq;
		var settings = DataTable.settings;
		var tables = $.map( settings, function (el, i) {
			return el.nTable;
		} );
	
		if ( ! mixed ) {
			return [];
		}
		else if ( mixed.nTable && mixed.oApi ) {
			// DataTables settings object
			return [ mixed ];
		}
		else if ( mixed.nodeName && mixed.nodeName.toLowerCase() === 'table' ) {
			// Table node
			idx = $.inArray( mixed, tables );
			return idx !== -1 ? [ settings[idx] ] : null;
		}
		else if ( mixed && typeof mixed.settings === 'function' ) {
			return mixed.settings().toArray();
		}
		else if ( typeof mixed === 'string' ) {
			// jQuery selector
			jq = $(mixed);
		}
		else if ( mixed instanceof $ ) {
			// jQuery object (also DataTables instance)
			jq = mixed;
		}
	
		if ( jq ) {
			return jq.map( function(i) {
				idx = $.inArray( this, tables );
				return idx !== -1 ? settings[idx] : null;
			} ).toArray();
		}
	};
	
	
	/**
	 * DataTables API class - used to control and interface with  one or more
	 * DataTables enhanced tables.
	 *
	 * The API class is heavily based on jQuery, presenting a chainable interface
	 * that you can use to interact with tables. Each instance of the API class has
	 * a "context" - i.e. the tables that it will operate on. This could be a single
	 * table, all tables on a page or a sub-set thereof.
	 *
	 * Additionally the API is designed to allow you to easily work with the data in
	 * the tables, retrieving and manipulating it as required. This is done by
	 * presenting the API class as an array like interface. The contents of the
	 * array depend upon the actions requested by each method (for example
	 * `rows().nodes()` will return an array of nodes, while `rows().data()` will
	 * return an array of objects or arrays depending upon your table's
	 * configuration). The API object has a number of array like methods (`push`,
	 * `pop`, `reverse` etc) as well as additional helper methods (`each`, `pluck`,
	 * `unique` etc) to assist your working with the data held in a table.
	 *
	 * Most methods (those which return an Api instance) are chainable, which means
	 * the return from a method call also has all of the methods available that the
	 * top level object had. For example, these two calls are equivalent:
	 *
	 *     // Not chained
	 *     api.row.add( {...} );
	 *     api.draw();
	 *
	 *     // Chained
	 *     api.row.add( {...} ).draw();
	 *
	 * @class DataTable.Api
	 * @param {array|object|string|jQuery} context DataTable identifier. This is
	 *   used to define which DataTables enhanced tables this API will operate on.
	 *   Can be one of:
	 *
	 *   * `string` - jQuery selector. Any DataTables' matching the given selector
	 *     with be found and used.
	 *   * `node` - `TABLE` node which has already been formed into a DataTable.
	 *   * `jQuery` - A jQuery object of `TABLE` nodes.
	 *   * `object` - DataTables settings object
	 * @param {array} [data] Data to initialise the Api instance with.
	 *
	 * @example
	 *   // Direct initialisation during DataTables construction
	 *   var api = $('#example').DataTable();
	 *
	 * @example
	 *   // Initialisation using a DataTables jQuery object
	 *   var api = $('#example').dataTable().api();
	 *
	 * @example
	 *   // Initialisation as a constructor
	 *   var api = new $.fn.DataTable.Api( 'table.dataTable' );
	 */
	_Api = function ( context, data )
	{
		if ( ! (this instanceof _Api) ) {
			return new _Api( context, data );
		}
	
		var settings = [];
		var ctxSettings = function ( o ) {
			var a = _toSettings( o );
			if ( a ) {
				settings = settings.concat( a );
			}
		};
	
		if ( $.isArray( context ) ) {
			for ( var i=0, ien=context.length ; i<ien ; i++ ) {
				ctxSettings( context[i] );
			}
		}
		else {
			ctxSettings( context );
		}
	
		// Remove duplicates
		this.context = _unique( settings );
	
		// Initial data
		if ( data ) {
			$.merge( this, data );
		}
	
		// selector
		this.selector = {
			rows: null,
			cols: null,
			opts: null
		};
	
		_Api.extend( this, this, __apiStruct );
	};
	
	DataTable.Api = _Api;
	
	// Don't destroy the existing prototype, just extend it. Required for jQuery 2's
	// isPlainObject.
	$.extend( _Api.prototype, {
		any: function ()
		{
			return this.count() !== 0;
		},
	
	
		concat:  __arrayProto.concat,
	
	
		context: [], // array of table settings objects
	
	
		count: function ()
		{
			return this.flatten().length;
		},
	
	
		each: function ( fn )
		{
			for ( var i=0, ien=this.length ; i<ien; i++ ) {
				fn.call( this, this[i], i, this );
			}
	
			return this;
		},
	
	
		eq: function ( idx )
		{
			var ctx = this.context;
	
			return ctx.length > idx ?
				new _Api( ctx[idx], this[idx] ) :
				null;
		},
	
	
		filter: function ( fn )
		{
			var a = [];
	
			if ( __arrayProto.filter ) {
				a = __arrayProto.filter.call( this, fn, this );
			}
			else {
				// Compatibility for browsers without EMCA-252-5 (JS 1.6)
				for ( var i=0, ien=this.length ; i<ien ; i++ ) {
					if ( fn.call( this, this[i], i, this ) ) {
						a.push( this[i] );
					}
				}
			}
	
			return new _Api( this.context, a );
		},
	
	
		flatten: function ()
		{
			var a = [];
			return new _Api( this.context, a.concat.apply( a, this.toArray() ) );
		},
	
	
		join:    __arrayProto.join,
	
	
		indexOf: __arrayProto.indexOf || function (obj, start)
		{
			for ( var i=(start || 0), ien=this.length ; i<ien ; i++ ) {
				if ( this[i] === obj ) {
					return i;
				}
			}
			return -1;
		},
	
		iterator: function ( flatten, type, fn, alwaysNew ) {
			var
				a = [], ret,
				i, ien, j, jen,
				context = this.context,
				rows, items, item,
				selector = this.selector;
	
			// Argument shifting
			if ( typeof flatten === 'string' ) {
				alwaysNew = fn;
				fn = type;
				type = flatten;
				flatten = false;
			}
	
			for ( i=0, ien=context.length ; i<ien ; i++ ) {
				var apiInst = new _Api( context[i] );
	
				if ( type === 'table' ) {
					ret = fn.call( apiInst, context[i], i );
	
					if ( ret !== undefined ) {
						a.push( ret );
					}
				}
				else if ( type === 'columns' || type === 'rows' ) {
					// this has same length as context - one entry for each table
					ret = fn.call( apiInst, context[i], this[i], i );
	
					if ( ret !== undefined ) {
						a.push( ret );
					}
				}
				else if ( type === 'column' || type === 'column-rows' || type === 'row' || type === 'cell' ) {
					// columns and rows share the same structure.
					// 'this' is an array of column indexes for each context
					items = this[i];
	
					if ( type === 'column-rows' ) {
						rows = _selector_row_indexes( context[i], selector.opts );
					}
	
					for ( j=0, jen=items.length ; j<jen ; j++ ) {
						item = items[j];
	
						if ( type === 'cell' ) {
							ret = fn.call( apiInst, context[i], item.row, item.column, i, j );
						}
						else {
							ret = fn.call( apiInst, context[i], item, i, j, rows );
						}
	
						if ( ret !== undefined ) {
							a.push( ret );
						}
					}
				}
			}
	
			if ( a.length || alwaysNew ) {
				var api = new _Api( context, flatten ? a.concat.apply( [], a ) : a );
				var apiSelector = api.selector;
				apiSelector.rows = selector.rows;
				apiSelector.cols = selector.cols;
				apiSelector.opts = selector.opts;
				return api;
			}
			return this;
		},
	
	
		lastIndexOf: __arrayProto.lastIndexOf || function (obj, start)
		{
			// Bit cheeky...
			return this.indexOf.apply( this.toArray.reverse(), arguments );
		},
	
	
		length:  0,
	
	
		map: function ( fn )
		{
			var a = [];
	
			if ( __arrayProto.map ) {
				a = __arrayProto.map.call( this, fn, this );
			}
			else {
				// Compatibility for browsers without EMCA-252-5 (JS 1.6)
				for ( var i=0, ien=this.length ; i<ien ; i++ ) {
					a.push( fn.call( this, this[i], i ) );
				}
			}
	
			return new _Api( this.context, a );
		},
	
	
		pluck: function ( prop )
		{
			return this.map( function ( el ) {
				return el[ prop ];
			} );
		},
	
		pop:     __arrayProto.pop,
	
	
		push:    __arrayProto.push,
	
	
		// Does not return an API instance
		reduce: __arrayProto.reduce || function ( fn, init )
		{
			return _fnReduce( this, fn, init, 0, this.length, 1 );
		},
	
	
		reduceRight: __arrayProto.reduceRight || function ( fn, init )
		{
			return _fnReduce( this, fn, init, this.length-1, -1, -1 );
		},
	
	
		reverse: __arrayProto.reverse,
	
	
		// Object with rows, columns and opts
		selector: null,
	
	
		shift:   __arrayProto.shift,
	
	
		slice: function () {
			return new _Api( this.context, this );
		},
	
	
		sort:    __arrayProto.sort, // ? name - order?
	
	
		splice:  __arrayProto.splice,
	
	
		toArray: function ()
		{
			return __arrayProto.slice.call( this );
		},
	
	
		to$: function ()
		{
			return $( this );
		},
	
	
		toJQuery: function ()
		{
			return $( this );
		},
	
	
		unique: function ()
		{
			return new _Api( this.context, _unique(this) );
		},
	
	
		unshift: __arrayProto.unshift
	} );
	
	
	_Api.extend = function ( scope, obj, ext )
	{
		// Only extend API instances and static properties of the API
		if ( ! ext.length || ! obj || ( ! (obj instanceof _Api) && ! obj.__dt_wrapper ) ) {
			return;
		}
	
		var
			i, ien,
			j, jen,
			struct, inner,
			methodScoping = function ( scope, fn, struc ) {
				return function () {
					var ret = fn.apply( scope, arguments );
	
					// Method extension
					_Api.extend( ret, ret, struc.methodExt );
					return ret;
				};
			};
	
		for ( i=0, ien=ext.length ; i<ien ; i++ ) {
			struct = ext[i];
	
			// Value
			obj[ struct.name ] = typeof struct.val === 'function' ?
				methodScoping( scope, struct.val, struct ) :
				$.isPlainObject( struct.val ) ?
					{} :
					struct.val;
	
			obj[ struct.name ].__dt_wrapper = true;
	
			// Property extension
			_Api.extend( scope, obj[ struct.name ], struct.propExt );
		}
	};
	
	
	// @todo - Is there need for an augment function?
	// _Api.augment = function ( inst, name )
	// {
	// 	// Find src object in the structure from the name
	// 	var parts = name.split('.');
	
	// 	_Api.extend( inst, obj );
	// };
	
	
	//     [
	//       {
	//         name:      'data'                -- string   - Property name
	//         val:       function () {},       -- function - Api method (or undefined if just an object
	//         methodExt: [ ... ],              -- array    - Array of Api object definitions to extend the method result
	//         propExt:   [ ... ]               -- array    - Array of Api object definitions to extend the property
	//       },
	//       {
	//         name:     'row'
	//         val:       {},
	//         methodExt: [ ... ],
	//         propExt:   [
	//           {
	//             name:      'data'
	//             val:       function () {},
	//             methodExt: [ ... ],
	//             propExt:   [ ... ]
	//           },
	//           ...
	//         ]
	//       }
	//     ]
	
	_Api.register = _api_register = function ( name, val )
	{
		if ( $.isArray( name ) ) {
			for ( var j=0, jen=name.length ; j<jen ; j++ ) {
				_Api.register( name[j], val );
			}
			return;
		}
	
		var
			i, ien,
			heir = name.split('.'),
			struct = __apiStruct,
			key, method;
	
		var find = function ( src, name ) {
			for ( var i=0, ien=src.length ; i<ien ; i++ ) {
				if ( src[i].name === name ) {
					return src[i];
				}
			}
			return null;
		};
	
		for ( i=0, ien=heir.length ; i<ien ; i++ ) {
			method = heir[i].indexOf('()') !== -1;
			key = method ?
				heir[i].replace('()', '') :
				heir[i];
	
			var src = find( struct, key );
			if ( ! src ) {
				src = {
					name:      key,
					val:       {},
					methodExt: [],
					propExt:   []
				};
				struct.push( src );
			}
	
			if ( i === ien-1 ) {
				src.val = val;
			}
			else {
				struct = method ?
					src.methodExt :
					src.propExt;
			}
		}
	};
	
	
	_Api.registerPlural = _api_registerPlural = function ( pluralName, singularName, val ) {
		_Api.register( pluralName, val );
	
		_Api.register( singularName, function () {
			var ret = val.apply( this, arguments );
	
			if ( ret === this ) {
				// Returned item is the API instance that was passed in, return it
				return this;
			}
			else if ( ret instanceof _Api ) {
				// New API instance returned, want the value from the first item
				// in the returned array for the singular result.
				return ret.length ?
					$.isArray( ret[0] ) ?
						new _Api( ret.context, ret[0] ) : // Array results are 'enhanced'
						ret[0] :
					undefined;
			}
	
			// Non-API return - just fire it back
			return ret;
		} );
	};
	
	
	/**
	 * Selector for HTML tables. Apply the given selector to the give array of
	 * DataTables settings objects.
	 *
	 * @param {string|integer} [selector] jQuery selector string or integer
	 * @param  {array} Array of DataTables settings objects to be filtered
	 * @return {array}
	 * @ignore
	 */
	var __table_selector = function ( selector, a )
	{
		// Integer is used to pick out a table by index
		if ( typeof selector === 'number' ) {
			return [ a[ selector ] ];
		}
	
		// Perform a jQuery selector on the table nodes
		var nodes = $.map( a, function (el, i) {
			return el.nTable;
		} );
	
		return $(nodes)
			.filter( selector )
			.map( function (i) {
				// Need to translate back from the table node to the settings
				var idx = $.inArray( this, nodes );
				return a[ idx ];
			} )
			.toArray();
	};
	
	
	
	/**
	 * Context selector for the API's context (i.e. the tables the API instance
	 * refers to.
	 *
	 * @name    DataTable.Api#tables
	 * @param {string|integer} [selector] Selector to pick which tables the iterator
	 *   should operate on. If not given, all tables in the current context are
	 *   used. This can be given as a jQuery selector (for example `':gt(0)'`) to
	 *   select multiple tables or as an integer to select a single table.
	 * @returns {DataTable.Api} Returns a new API instance if a selector is given.
	 */
	_api_register( 'tables()', function ( selector ) {
		// A new instance is created if there was a selector specified
		return selector ?
			new _Api( __table_selector( selector, this.context ) ) :
			this;
	} );
	
	
	_api_register( 'table()', function ( selector ) {
		var tables = this.tables( selector );
		var ctx = tables.context;
	
		// Truncate to the first matched table
		return ctx.length ?
			new _Api( ctx[0] ) :
			tables;
	} );
	
	
	_api_registerPlural( 'tables().nodes()', 'table().node()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTable;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().body()', 'table().body()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTBody;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().header()', 'table().header()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTHead;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().footer()', 'table().footer()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTFoot;
		}, 1 );
	} );
	
	
	_api_registerPlural( 'tables().containers()', 'table().container()' , function () {
		return this.iterator( 'table', function ( ctx ) {
			return ctx.nTableWrapper;
		}, 1 );
	} );
	
	
	
	/**
	 * Redraw the tables in the current context.
	 */
	_api_register( 'draw()', function ( paging ) {
		return this.iterator( 'table', function ( settings ) {
			if ( paging === 'page' ) {
				_fnDraw( settings );
			}
			else {
				if ( typeof paging === 'string' ) {
					paging = paging === 'full-hold' ?
						false :
						true;
				}
	
				_fnReDraw( settings, paging===false );
			}
		} );
	} );
	
	
	
	/**
	 * Get the current page index.
	 *
	 * @return {integer} Current page index (zero based)
	 *//**
	 * Set the current page.
	 *
	 * Note that if you attempt to show a page which does not exist, DataTables will
	 * not throw an error, but rather reset the paging.
	 *
	 * @param {integer|string} action The paging action to take. This can be one of:
	 *  * `integer` - The page index to jump to
	 *  * `string` - An action to take:
	 *    * `first` - Jump to first page.
	 *    * `next` - Jump to the next page
	 *    * `previous` - Jump to previous page
	 *    * `last` - Jump to the last page.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'page()', function ( action ) {
		if ( action === undefined ) {
			return this.page.info().page; // not an expensive call
		}
	
		// else, have an action to take on all tables
		return this.iterator( 'table', function ( settings ) {
			_fnPageChange( settings, action );
		} );
	} );
	
	
	/**
	 * Paging information for the first table in the current context.
	 *
	 * If you require paging information for another table, use the `table()` method
	 * with a suitable selector.
	 *
	 * @return {object} Object with the following properties set:
	 *  * `page` - Current page index (zero based - i.e. the first page is `0`)
	 *  * `pages` - Total number of pages
	 *  * `start` - Display index for the first record shown on the current page
	 *  * `end` - Display index for the last record shown on the current page
	 *  * `length` - Display length (number of records). Note that generally `start
	 *    + length = end`, but this is not always true, for example if there are
	 *    only 2 records to show on the final page, with a length of 10.
	 *  * `recordsTotal` - Full data set length
	 *  * `recordsDisplay` - Data set length once the current filtering criterion
	 *    are applied.
	 */
	_api_register( 'page.info()', function ( action ) {
		if ( this.context.length === 0 ) {
			return undefined;
		}
	
		var
			settings   = this.context[0],
			start      = settings._iDisplayStart,
			len        = settings.oFeatures.bPaginate ? settings._iDisplayLength : -1,
			visRecords = settings.fnRecordsDisplay(),
			all        = len === -1;
	
		return {
			"page":           all ? 0 : Math.floor( start / len ),
			"pages":          all ? 1 : Math.ceil( visRecords / len ),
			"start":          start,
			"end":            settings.fnDisplayEnd(),
			"length":         len,
			"recordsTotal":   settings.fnRecordsTotal(),
			"recordsDisplay": visRecords,
			"serverSide":     _fnDataSource( settings ) === 'ssp'
		};
	} );
	
	
	/**
	 * Get the current page length.
	 *
	 * @return {integer} Current page length. Note `-1` indicates that all records
	 *   are to be shown.
	 *//**
	 * Set the current page length.
	 *
	 * @param {integer} Page length to set. Use `-1` to show all records.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'page.len()', function ( len ) {
		// Note that we can't call this function 'length()' because `length`
		// is a Javascript property of functions which defines how many arguments
		// the function expects.
		if ( len === undefined ) {
			return this.context.length !== 0 ?
				this.context[0]._iDisplayLength :
				undefined;
		}
	
		// else, set the page length
		return this.iterator( 'table', function ( settings ) {
			_fnLengthChange( settings, len );
		} );
	} );
	
	
	
	var __reload = function ( settings, holdPosition, callback ) {
		// Use the draw event to trigger a callback
		if ( callback ) {
			var api = new _Api( settings );
	
			api.one( 'draw', function () {
				callback( api.ajax.json() );
			} );
		}
	
		if ( _fnDataSource( settings ) == 'ssp' ) {
			_fnReDraw( settings, holdPosition );
		}
		else {
			_fnProcessingDisplay( settings, true );
	
			// Cancel an existing request
			var xhr = settings.jqXHR;
			if ( xhr && xhr.readyState !== 4 ) {
				xhr.abort();
			}
	
			// Trigger xhr
			_fnBuildAjax( settings, [], function( json ) {
				_fnClearTable( settings );
	
				var data = _fnAjaxDataSrc( settings, json );
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					_fnAddData( settings, data[i] );
				}
	
				_fnReDraw( settings, holdPosition );
				_fnProcessingDisplay( settings, false );
			} );
		}
	};
	
	
	/**
	 * Get the JSON response from the last Ajax request that DataTables made to the
	 * server. Note that this returns the JSON from the first table in the current
	 * context.
	 *
	 * @return {object} JSON received from the server.
	 */
	_api_register( 'ajax.json()', function () {
		var ctx = this.context;
	
		if ( ctx.length > 0 ) {
			return ctx[0].json;
		}
	
		// else return undefined;
	} );
	
	
	/**
	 * Get the data submitted in the last Ajax request
	 */
	_api_register( 'ajax.params()', function () {
		var ctx = this.context;
	
		if ( ctx.length > 0 ) {
			return ctx[0].oAjaxData;
		}
	
		// else return undefined;
	} );
	
	
	/**
	 * Reload tables from the Ajax data source. Note that this function will
	 * automatically re-draw the table when the remote data has been loaded.
	 *
	 * @param {boolean} [reset=true] Reset (default) or hold the current paging
	 *   position. A full re-sort and re-filter is performed when this method is
	 *   called, which is why the pagination reset is the default action.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.reload()', function ( callback, resetPaging ) {
		return this.iterator( 'table', function (settings) {
			__reload( settings, resetPaging===false, callback );
		} );
	} );
	
	
	/**
	 * Get the current Ajax URL. Note that this returns the URL from the first
	 * table in the current context.
	 *
	 * @return {string} Current Ajax source URL
	 *//**
	 * Set the Ajax URL. Note that this will set the URL for all tables in the
	 * current context.
	 *
	 * @param {string} url URL to set.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.url()', function ( url ) {
		var ctx = this.context;
	
		if ( url === undefined ) {
			// get
			if ( ctx.length === 0 ) {
				return undefined;
			}
			ctx = ctx[0];
	
			return ctx.ajax ?
				$.isPlainObject( ctx.ajax ) ?
					ctx.ajax.url :
					ctx.ajax :
				ctx.sAjaxSource;
		}
	
		// set
		return this.iterator( 'table', function ( settings ) {
			if ( $.isPlainObject( settings.ajax ) ) {
				settings.ajax.url = url;
			}
			else {
				settings.ajax = url;
			}
			// No need to consider sAjaxSource here since DataTables gives priority
			// to `ajax` over `sAjaxSource`. So setting `ajax` here, renders any
			// value of `sAjaxSource` redundant.
		} );
	} );
	
	
	/**
	 * Load data from the newly set Ajax URL. Note that this method is only
	 * available when `ajax.url()` is used to set a URL. Additionally, this method
	 * has the same effect as calling `ajax.reload()` but is provided for
	 * convenience when setting a new URL. Like `ajax.reload()` it will
	 * automatically redraw the table once the remote data has been loaded.
	 *
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'ajax.url().load()', function ( callback, resetPaging ) {
		// Same as a reload, but makes sense to present it for easy access after a
		// url change
		return this.iterator( 'table', function ( ctx ) {
			__reload( ctx, resetPaging===false, callback );
		} );
	} );
	
	
	
	
	var _selector_run = function ( type, selector, selectFn, settings, opts )
	{
		var
			out = [], res,
			a, i, ien, j, jen,
			selectorType = typeof selector;
	
		// Can't just check for isArray here, as an API or jQuery instance might be
		// given with their array like look
		if ( ! selector || selectorType === 'string' || selectorType === 'function' || selector.length === undefined ) {
			selector = [ selector ];
		}
	
		for ( i=0, ien=selector.length ; i<ien ; i++ ) {
			// Only split on simple strings - complex expressions will be jQuery selectors
			a = selector[i] && selector[i].split && ! selector[i].match(/[\[\(:]/) ?
				selector[i].split(',') :
				[ selector[i] ];
	
			for ( j=0, jen=a.length ; j<jen ; j++ ) {
				res = selectFn( typeof a[j] === 'string' ? $.trim(a[j]) : a[j] );
	
				if ( res && res.length ) {
					out = out.concat( res );
				}
			}
		}
	
		// selector extensions
		var ext = _ext.selector[ type ];
		if ( ext.length ) {
			for ( i=0, ien=ext.length ; i<ien ; i++ ) {
				out = ext[i]( settings, opts, out );
			}
		}
	
		return _unique( out );
	};
	
	
	var _selector_opts = function ( opts )
	{
		if ( ! opts ) {
			opts = {};
		}
	
		// Backwards compatibility for 1.9- which used the terminology filter rather
		// than search
		if ( opts.filter && opts.search === undefined ) {
			opts.search = opts.filter;
		}
	
		return $.extend( {
			search: 'none',
			order: 'current',
			page: 'all'
		}, opts );
	};
	
	
	var _selector_first = function ( inst )
	{
		// Reduce the API instance to the first item found
		for ( var i=0, ien=inst.length ; i<ien ; i++ ) {
			if ( inst[i].length > 0 ) {
				// Assign the first element to the first item in the instance
				// and truncate the instance and context
				inst[0] = inst[i];
				inst[0].length = 1;
				inst.length = 1;
				inst.context = [ inst.context[i] ];
	
				return inst;
			}
		}
	
		// Not found - return an empty instance
		inst.length = 0;
		return inst;
	};
	
	
	var _selector_row_indexes = function ( settings, opts )
	{
		var
			i, ien, tmp, a=[],
			displayFiltered = settings.aiDisplay,
			displayMaster = settings.aiDisplayMaster;
	
		var
			search = opts.search,  // none, applied, removed
			order  = opts.order,   // applied, current, index (original - compatibility with 1.9)
			page   = opts.page;    // all, current
	
		if ( _fnDataSource( settings ) == 'ssp' ) {
			// In server-side processing mode, most options are irrelevant since
			// rows not shown don't exist and the index order is the applied order
			// Removed is a special case - for consistency just return an empty
			// array
			return search === 'removed' ?
				[] :
				_range( 0, displayMaster.length );
		}
		else if ( page == 'current' ) {
			// Current page implies that order=current and fitler=applied, since it is
			// fairly senseless otherwise, regardless of what order and search actually
			// are
			for ( i=settings._iDisplayStart, ien=settings.fnDisplayEnd() ; i<ien ; i++ ) {
				a.push( displayFiltered[i] );
			}
		}
		else if ( order == 'current' || order == 'applied' ) {
			a = search == 'none' ?
				displayMaster.slice() :                      // no search
				search == 'applied' ?
					displayFiltered.slice() :                // applied search
					$.map( displayMaster, function (el, i) { // removed search
						return $.inArray( el, displayFiltered ) === -1 ? el : null;
					} );
		}
		else if ( order == 'index' || order == 'original' ) {
			for ( i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
				if ( search == 'none' ) {
					a.push( i );
				}
				else { // applied | removed
					tmp = $.inArray( i, displayFiltered );
	
					if ((tmp === -1 && search == 'removed') ||
						(tmp >= 0   && search == 'applied') )
					{
						a.push( i );
					}
				}
			}
		}
	
		return a;
	};
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Rows
	 *
	 * {}          - no selector - use all available rows
	 * {integer}   - row aoData index
	 * {node}      - TR node
	 * {string}    - jQuery selector to apply to the TR elements
	 * {array}     - jQuery array of nodes, or simply an array of TR nodes
	 *
	 */
	
	
	var __row_selector = function ( settings, selector, opts )
	{
		var rows;
		var run = function ( sel ) {
			var selInt = _intVal( sel );
			var i, ien;
	
			// Short cut - selector is a number and no options provided (default is
			// all records, so no need to check if the index is in there, since it
			// must be - dev error if the index doesn't exist).
			if ( selInt !== null && ! opts ) {
				return [ selInt ];
			}
	
			if ( ! rows ) {
				rows = _selector_row_indexes( settings, opts );
			}
	
			if ( selInt !== null && $.inArray( selInt, rows ) !== -1 ) {
				// Selector - integer
				return [ selInt ];
			}
			else if ( sel === null || sel === undefined || sel === '' ) {
				// Selector - none
				return rows;
			}
	
			// Selector - function
			if ( typeof sel === 'function' ) {
				return $.map( rows, function (idx) {
					var row = settings.aoData[ idx ];
					return sel( idx, row._aData, row.nTr ) ? idx : null;
				} );
			}
	
			// Get nodes in the order from the `rows` array with null values removed
			var nodes = _removeEmpty(
				_pluck_order( settings.aoData, rows, 'nTr' )
			);
	
			// Selector - node
			if ( sel.nodeName ) {
				if ( sel._DT_RowIndex !== undefined ) {
					return [ sel._DT_RowIndex ]; // Property added by DT for fast lookup
				}
				else if ( sel._DT_CellIndex ) {
					return [ sel._DT_CellIndex.row ];
				}
				else {
					var host = $(sel).closest('*[data-dt-row]');
					return host.length ?
						[ host.data('dt-row') ] :
						[];
				}
			}
	
			// ID selector. Want to always be able to select rows by id, regardless
			// of if the tr element has been created or not, so can't rely upon
			// jQuery here - hence a custom implementation. This does not match
			// Sizzle's fast selector or HTML4 - in HTML5 the ID can be anything,
			// but to select it using a CSS selector engine (like Sizzle or
			// querySelect) it would need to need to be escaped for some characters.
			// DataTables simplifies this for row selectors since you can select
			// only a row. A # indicates an id any anything that follows is the id -
			// unescaped.
			if ( typeof sel === 'string' && sel.charAt(0) === '#' ) {
				// get row index from id
				var rowObj = settings.aIds[ sel.replace( /^#/, '' ) ];
				if ( rowObj !== undefined ) {
					return [ rowObj.idx ];
				}
	
				// need to fall through to jQuery in case there is DOM id that
				// matches
			}
	
			// Selector - jQuery selector string, array of nodes or jQuery object/
			// As jQuery's .filter() allows jQuery objects to be passed in filter,
			// it also allows arrays, so this will cope with all three options
			return $(nodes)
				.filter( sel )
				.map( function () {
					return this._DT_RowIndex;
				} )
				.toArray();
		};
	
		return _selector_run( 'row', selector, run, settings, opts );
	};
	
	
	_api_register( 'rows()', function ( selector, opts ) {
		// argument shifting
		if ( selector === undefined ) {
			selector = '';
		}
		else if ( $.isPlainObject( selector ) ) {
			opts = selector;
			selector = '';
		}
	
		opts = _selector_opts( opts );
	
		var inst = this.iterator( 'table', function ( settings ) {
			return __row_selector( settings, selector, opts );
		}, 1 );
	
		// Want argument shifting here and in __row_selector?
		inst.selector.rows = selector;
		inst.selector.opts = opts;
	
		return inst;
	} );
	
	_api_register( 'rows().nodes()', function () {
		return this.iterator( 'row', function ( settings, row ) {
			return settings.aoData[ row ].nTr || undefined;
		}, 1 );
	} );
	
	_api_register( 'rows().data()', function () {
		return this.iterator( true, 'rows', function ( settings, rows ) {
			return _pluck_order( settings.aoData, rows, '_aData' );
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().cache()', 'row().cache()', function ( type ) {
		return this.iterator( 'row', function ( settings, row ) {
			var r = settings.aoData[ row ];
			return type === 'search' ? r._aFilterData : r._aSortData;
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().invalidate()', 'row().invalidate()', function ( src ) {
		return this.iterator( 'row', function ( settings, row ) {
			_fnInvalidate( settings, row, src );
		} );
	} );
	
	_api_registerPlural( 'rows().indexes()', 'row().index()', function () {
		return this.iterator( 'row', function ( settings, row ) {
			return row;
		}, 1 );
	} );
	
	_api_registerPlural( 'rows().ids()', 'row().id()', function ( hash ) {
		var a = [];
		var context = this.context;
	
		// `iterator` will drop undefined values, but in this case we want them
		for ( var i=0, ien=context.length ; i<ien ; i++ ) {
			for ( var j=0, jen=this[i].length ; j<jen ; j++ ) {
				var id = context[i].rowIdFn( context[i].aoData[ this[i][j] ]._aData );
				a.push( (hash === true ? '#' : '' )+ id );
			}
		}
	
		return new _Api( context, a );
	} );
	
	_api_registerPlural( 'rows().remove()', 'row().remove()', function () {
		var that = this;
	
		this.iterator( 'row', function ( settings, row, thatIdx ) {
			var data = settings.aoData;
			var rowData = data[ row ];
			var i, ien, j, jen;
			var loopRow, loopCells;
	
			data.splice( row, 1 );
	
			// Update the cached indexes
			for ( i=0, ien=data.length ; i<ien ; i++ ) {
				loopRow = data[i];
				loopCells = loopRow.anCells;
	
				// Rows
				if ( loopRow.nTr !== null ) {
					loopRow.nTr._DT_RowIndex = i;
				}
	
				// Cells
				if ( loopCells !== null ) {
					for ( j=0, jen=loopCells.length ; j<jen ; j++ ) {
						loopCells[j]._DT_CellIndex.row = i;
					}
				}
			}
	
			// Delete from the display arrays
			_fnDeleteIndex( settings.aiDisplayMaster, row );
			_fnDeleteIndex( settings.aiDisplay, row );
			_fnDeleteIndex( that[ thatIdx ], row, false ); // maintain local indexes
	
			// For server-side processing tables - subtract the deleted row from the count
			if ( settings._iRecordsDisplay > 0 ) {
				settings._iRecordsDisplay--;
			}
	
			// Check for an 'overflow' they case for displaying the table
			_fnLengthOverflow( settings );
	
			// Remove the row's ID reference if there is one
			var id = settings.rowIdFn( rowData._aData );
			if ( id !== undefined ) {
				delete settings.aIds[ id ];
			}
		} );
	
		this.iterator( 'table', function ( settings ) {
			for ( var i=0, ien=settings.aoData.length ; i<ien ; i++ ) {
				settings.aoData[i].idx = i;
			}
		} );
	
		return this;
	} );
	
	
	_api_register( 'rows.add()', function ( rows ) {
		var newRows = this.iterator( 'table', function ( settings ) {
				var row, i, ien;
				var out = [];
	
				for ( i=0, ien=rows.length ; i<ien ; i++ ) {
					row = rows[i];
	
					if ( row.nodeName && row.nodeName.toUpperCase() === 'TR' ) {
						out.push( _fnAddTr( settings, row )[0] );
					}
					else {
						out.push( _fnAddData( settings, row ) );
					}
				}
	
				return out;
			}, 1 );
	
		// Return an Api.rows() extended instance, so rows().nodes() etc can be used
		var modRows = this.rows( -1 );
		modRows.pop();
		$.merge( modRows, newRows );
	
		return modRows;
	} );
	
	
	
	
	
	/**
	 *
	 */
	_api_register( 'row()', function ( selector, opts ) {
		return _selector_first( this.rows( selector, opts ) );
	} );
	
	
	_api_register( 'row().data()', function ( data ) {
		var ctx = this.context;
	
		if ( data === undefined ) {
			// Get
			return ctx.length && this.length ?
				ctx[0].aoData[ this[0] ]._aData :
				undefined;
		}
	
		// Set
		ctx[0].aoData[ this[0] ]._aData = data;
	
		// Automatically invalidate
		_fnInvalidate( ctx[0], this[0], 'data' );
	
		return this;
	} );
	
	
	_api_register( 'row().node()', function () {
		var ctx = this.context;
	
		return ctx.length && this.length ?
			ctx[0].aoData[ this[0] ].nTr || null :
			null;
	} );
	
	
	_api_register( 'row.add()', function ( row ) {
		// Allow a jQuery object to be passed in - only a single row is added from
		// it though - the first element in the set
		if ( row instanceof $ && row.length ) {
			row = row[0];
		}
	
		var rows = this.iterator( 'table', function ( settings ) {
			if ( row.nodeName && row.nodeName.toUpperCase() === 'TR' ) {
				return _fnAddTr( settings, row )[0];
			}
			return _fnAddData( settings, row );
		} );
	
		// Return an Api.rows() extended instance, with the newly added row selected
		return this.row( rows[0] );
	} );
	
	
	
	var __details_add = function ( ctx, row, data, klass )
	{
		// Convert to array of TR elements
		var rows = [];
		var addRow = function ( r, k ) {
			// Recursion to allow for arrays of jQuery objects
			if ( $.isArray( r ) || r instanceof $ ) {
				for ( var i=0, ien=r.length ; i<ien ; i++ ) {
					addRow( r[i], k );
				}
				return;
			}
	
			// If we get a TR element, then just add it directly - up to the dev
			// to add the correct number of columns etc
			if ( r.nodeName && r.nodeName.toLowerCase() === 'tr' ) {
				rows.push( r );
			}
			else {
				// Otherwise create a row with a wrapper
				var created = $('<tr><td/></tr>').addClass( k );
				$('td', created)
					.addClass( k )
					.html( r )
					[0].colSpan = _fnVisbleColumns( ctx );
	
				rows.push( created[0] );
			}
		};
	
		addRow( data, klass );
	
		if ( row._details ) {
			row._details.detach();
		}
	
		row._details = $(rows);
	
		// If the children were already shown, that state should be retained
		if ( row._detailsShow ) {
			row._details.insertAfter( row.nTr );
		}
	};
	
	
	var __details_remove = function ( api, idx )
	{
		var ctx = api.context;
	
		if ( ctx.length ) {
			var row = ctx[0].aoData[ idx !== undefined ? idx : api[0] ];
	
			if ( row && row._details ) {
				row._details.remove();
	
				row._detailsShow = undefined;
				row._details = undefined;
			}
		}
	};
	
	
	var __details_display = function ( api, show ) {
		var ctx = api.context;
	
		if ( ctx.length && api.length ) {
			var row = ctx[0].aoData[ api[0] ];
	
			if ( row._details ) {
				row._detailsShow = show;
	
				if ( show ) {
					row._details.insertAfter( row.nTr );
				}
				else {
					row._details.detach();
				}
	
				__details_events( ctx[0] );
			}
		}
	};
	
	
	var __details_events = function ( settings )
	{
		var api = new _Api( settings );
		var namespace = '.dt.DT_details';
		var drawEvent = 'draw'+namespace;
		var colvisEvent = 'column-visibility'+namespace;
		var destroyEvent = 'destroy'+namespace;
		var data = settings.aoData;
	
		api.off( drawEvent +' '+ colvisEvent +' '+ destroyEvent );
	
		if ( _pluck( data, '_details' ).length > 0 ) {
			// On each draw, insert the required elements into the document
			api.on( drawEvent, function ( e, ctx ) {
				if ( settings !== ctx ) {
					return;
				}
	
				api.rows( {page:'current'} ).eq(0).each( function (idx) {
					// Internal data grab
					var row = data[ idx ];
	
					if ( row._detailsShow ) {
						row._details.insertAfter( row.nTr );
					}
				} );
			} );
	
			// Column visibility change - update the colspan
			api.on( colvisEvent, function ( e, ctx, idx, vis ) {
				if ( settings !== ctx ) {
					return;
				}
	
				// Update the colspan for the details rows (note, only if it already has
				// a colspan)
				var row, visible = _fnVisbleColumns( ctx );
	
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					row = data[i];
	
					if ( row._details ) {
						row._details.children('td[colspan]').attr('colspan', visible );
					}
				}
			} );
	
			// Table destroyed - nuke any child rows
			api.on( destroyEvent, function ( e, ctx ) {
				if ( settings !== ctx ) {
					return;
				}
	
				for ( var i=0, ien=data.length ; i<ien ; i++ ) {
					if ( data[i]._details ) {
						__details_remove( api, i );
					}
				}
			} );
		}
	};
	
	// Strings for the method names to help minification
	var _emp = '';
	var _child_obj = _emp+'row().child';
	var _child_mth = _child_obj+'()';
	
	// data can be:
	//  tr
	//  string
	//  jQuery or array of any of the above
	_api_register( _child_mth, function ( data, klass ) {
		var ctx = this.context;
	
		if ( data === undefined ) {
			// get
			return ctx.length && this.length ?
				ctx[0].aoData[ this[0] ]._details :
				undefined;
		}
		else if ( data === true ) {
			// show
			this.child.show();
		}
		else if ( data === false ) {
			// remove
			__details_remove( this );
		}
		else if ( ctx.length && this.length ) {
			// set
			__details_add( ctx[0], ctx[0].aoData[ this[0] ], data, klass );
		}
	
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.show()',
		_child_mth+'.show()' // only when `child()` was called with parameters (without
	], function ( show ) {   // it returns an object and this method is not executed)
		__details_display( this, true );
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.hide()',
		_child_mth+'.hide()' // only when `child()` was called with parameters (without
	], function () {         // it returns an object and this method is not executed)
		__details_display( this, false );
		return this;
	} );
	
	
	_api_register( [
		_child_obj+'.remove()',
		_child_mth+'.remove()' // only when `child()` was called with parameters (without
	], function () {           // it returns an object and this method is not executed)
		__details_remove( this );
		return this;
	} );
	
	
	_api_register( _child_obj+'.isShown()', function () {
		var ctx = this.context;
	
		if ( ctx.length && this.length ) {
			// _detailsShown as false or undefined will fall through to return false
			return ctx[0].aoData[ this[0] ]._detailsShow || false;
		}
		return false;
	} );
	
	
	
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Columns
	 *
	 * {integer}           - column index (>=0 count from left, <0 count from right)
	 * "{integer}:visIdx"  - visible column index (i.e. translate to column index)  (>=0 count from left, <0 count from right)
	 * "{integer}:visible" - alias for {integer}:visIdx  (>=0 count from left, <0 count from right)
	 * "{string}:name"     - column name
	 * "{string}"          - jQuery selector on column header nodes
	 *
	 */
	
	// can be an array of these items, comma separated list, or an array of comma
	// separated lists
	
	var __re_column_selector = /^([^:]+):(name|visIdx|visible)$/;
	
	
	// r1 and r2 are redundant - but it means that the parameters match for the
	// iterator callback in columns().data()
	var __columnData = function ( settings, column, r1, r2, rows ) {
		var a = [];
		for ( var row=0, ien=rows.length ; row<ien ; row++ ) {
			a.push( _fnGetCellData( settings, rows[row], column ) );
		}
		return a;
	};
	
	
	var __column_selector = function ( settings, selector, opts )
	{
		var
			columns = settings.aoColumns,
			names = _pluck( columns, 'sName' ),
			nodes = _pluck( columns, 'nTh' );
	
		var run = function ( s ) {
			var selInt = _intVal( s );
	
			// Selector - all
			if ( s === '' ) {
				return _range( columns.length );
			}
	
			// Selector - index
			if ( selInt !== null ) {
				return [ selInt >= 0 ?
					selInt : // Count from left
					columns.length + selInt // Count from right (+ because its a negative value)
				];
			}
	
			// Selector = function
			if ( typeof s === 'function' ) {
				var rows = _selector_row_indexes( settings, opts );
	
				return $.map( columns, function (col, idx) {
					return s(
							idx,
							__columnData( settings, idx, 0, 0, rows ),
							nodes[ idx ]
						) ? idx : null;
				} );
			}
	
			// jQuery or string selector
			var match = typeof s === 'string' ?
				s.match( __re_column_selector ) :
				'';
	
			if ( match ) {
				switch( match[2] ) {
					case 'visIdx':
					case 'visible':
						var idx = parseInt( match[1], 10 );
						// Visible index given, convert to column index
						if ( idx < 0 ) {
							// Counting from the right
							var visColumns = $.map( columns, function (col,i) {
								return col.bVisible ? i : null;
							} );
							return [ visColumns[ visColumns.length + idx ] ];
						}
						// Counting from the left
						return [ _fnVisibleToColumnIndex( settings, idx ) ];
	
					case 'name':
						// match by name. `names` is column index complete and in order
						return $.map( names, function (name, i) {
							return name === match[1] ? i : null;
						} );
	
					default:
						return [];
				}
			}
	
			// Cell in the table body
			if ( s.nodeName && s._DT_CellIndex ) {
				return [ s._DT_CellIndex.column ];
			}
	
			// jQuery selector on the TH elements for the columns
			var jqResult = $( nodes )
				.filter( s )
				.map( function () {
					return $.inArray( this, nodes ); // `nodes` is column index complete and in order
				} )
				.toArray();
	
			if ( jqResult.length || ! s.nodeName ) {
				return jqResult;
			}
	
			// Otherwise a node which might have a `dt-column` data attribute, or be
			// a child or such an element
			var host = $(s).closest('*[data-dt-column]');
			return host.length ?
				[ host.data('dt-column') ] :
				[];
		};
	
		return _selector_run( 'column', selector, run, settings, opts );
	};
	
	
	var __setColumnVis = function ( settings, column, vis ) {
		var
			cols = settings.aoColumns,
			col  = cols[ column ],
			data = settings.aoData,
			row, cells, i, ien, tr;
	
		// Get
		if ( vis === undefined ) {
			return col.bVisible;
		}
	
		// Set
		// No change
		if ( col.bVisible === vis ) {
			return;
		}
	
		if ( vis ) {
			// Insert column
			// Need to decide if we should use appendChild or insertBefore
			var insertBefore = $.inArray( true, _pluck(cols, 'bVisible'), column+1 );
	
			for ( i=0, ien=data.length ; i<ien ; i++ ) {
				tr = data[i].nTr;
				cells = data[i].anCells;
	
				if ( tr ) {
					// insertBefore can act like appendChild if 2nd arg is null
					tr.insertBefore( cells[ column ], cells[ insertBefore ] || null );
				}
			}
		}
		else {
			// Remove column
			$( _pluck( settings.aoData, 'anCells', column ) ).detach();
		}
	
		// Common actions
		col.bVisible = vis;
		_fnDrawHead( settings, settings.aoHeader );
		_fnDrawHead( settings, settings.aoFooter );
	
		_fnSaveState( settings );
	};
	
	
	_api_register( 'columns()', function ( selector, opts ) {
		// argument shifting
		if ( selector === undefined ) {
			selector = '';
		}
		else if ( $.isPlainObject( selector ) ) {
			opts = selector;
			selector = '';
		}
	
		opts = _selector_opts( opts );
	
		var inst = this.iterator( 'table', function ( settings ) {
			return __column_selector( settings, selector, opts );
		}, 1 );
	
		// Want argument shifting here and in _row_selector?
		inst.selector.cols = selector;
		inst.selector.opts = opts;
	
		return inst;
	} );
	
	_api_registerPlural( 'columns().header()', 'column().header()', function ( selector, opts ) {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].nTh;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().footer()', 'column().footer()', function ( selector, opts ) {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].nTf;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().data()', 'column().data()', function () {
		return this.iterator( 'column-rows', __columnData, 1 );
	} );
	
	_api_registerPlural( 'columns().dataSrc()', 'column().dataSrc()', function () {
		return this.iterator( 'column', function ( settings, column ) {
			return settings.aoColumns[column].mData;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().cache()', 'column().cache()', function ( type ) {
		return this.iterator( 'column-rows', function ( settings, column, i, j, rows ) {
			return _pluck_order( settings.aoData, rows,
				type === 'search' ? '_aFilterData' : '_aSortData', column
			);
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().nodes()', 'column().nodes()', function () {
		return this.iterator( 'column-rows', function ( settings, column, i, j, rows ) {
			return _pluck_order( settings.aoData, rows, 'anCells', column ) ;
		}, 1 );
	} );
	
	_api_registerPlural( 'columns().visible()', 'column().visible()', function ( vis, calc ) {
		var ret = this.iterator( 'column', function ( settings, column ) {
			if ( vis === undefined ) {
				return settings.aoColumns[ column ].bVisible;
			} // else
			__setColumnVis( settings, column, vis );
		} );
	
		// Group the column visibility changes
		if ( vis !== undefined ) {
			// Second loop once the first is done for events
			this.iterator( 'column', function ( settings, column ) {
				_fnCallbackFire( settings, null, 'column-visibility', [settings, column, vis, calc] );
			} );
	
			if ( calc === undefined || calc ) {
				this.columns.adjust();
			}
		}
	
		return ret;
	} );
	
	_api_registerPlural( 'columns().indexes()', 'column().index()', function ( type ) {
		return this.iterator( 'column', function ( settings, column ) {
			return type === 'visible' ?
				_fnColumnIndexToVisible( settings, column ) :
				column;
		}, 1 );
	} );
	
	_api_register( 'columns.adjust()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnAdjustColumnSizing( settings );
		}, 1 );
	} );
	
	_api_register( 'column.index()', function ( type, idx ) {
		if ( this.context.length !== 0 ) {
			var ctx = this.context[0];
	
			if ( type === 'fromVisible' || type === 'toData' ) {
				return _fnVisibleToColumnIndex( ctx, idx );
			}
			else if ( type === 'fromData' || type === 'toVisible' ) {
				return _fnColumnIndexToVisible( ctx, idx );
			}
		}
	} );
	
	_api_register( 'column()', function ( selector, opts ) {
		return _selector_first( this.columns( selector, opts ) );
	} );
	
	
	
	var __cell_selector = function ( settings, selector, opts )
	{
		var data = settings.aoData;
		var rows = _selector_row_indexes( settings, opts );
		var cells = _removeEmpty( _pluck_order( data, rows, 'anCells' ) );
		var allCells = $( [].concat.apply([], cells) );
		var row;
		var columns = settings.aoColumns.length;
		var a, i, ien, j, o, host;
	
		var run = function ( s ) {
			var fnSelector = typeof s === 'function';
	
			if ( s === null || s === undefined || fnSelector ) {
				// All cells and function selectors
				a = [];
	
				for ( i=0, ien=rows.length ; i<ien ; i++ ) {
					row = rows[i];
	
					for ( j=0 ; j<columns ; j++ ) {
						o = {
							row: row,
							column: j
						};
	
						if ( fnSelector ) {
							// Selector - function
							host = data[ row ];
	
							if ( s( o, _fnGetCellData(settings, row, j), host.anCells ? host.anCells[j] : null ) ) {
								a.push( o );
							}
						}
						else {
							// Selector - all
							a.push( o );
						}
					}
				}
	
				return a;
			}
			
			// Selector - index
			if ( $.isPlainObject( s ) ) {
				return [s];
			}
	
			// Selector - jQuery filtered cells
			var jqResult = allCells
				.filter( s )
				.map( function (i, el) {
					return { // use a new object, in case someone changes the values
						row:    el._DT_CellIndex.row,
						column: el._DT_CellIndex.column
	 				};
				} )
				.toArray();
	
			if ( jqResult.length || ! s.nodeName ) {
				return jqResult;
			}
	
			// Otherwise the selector is a node, and there is one last option - the
			// element might be a child of an element which has dt-row and dt-column
			// data attributes
			host = $(s).closest('*[data-dt-row]');
			return host.length ?
				[ {
					row: host.data('dt-row'),
					column: host.data('dt-column')
				} ] :
				[];
		};
	
		return _selector_run( 'cell', selector, run, settings, opts );
	};
	
	
	
	
	_api_register( 'cells()', function ( rowSelector, columnSelector, opts ) {
		// Argument shifting
		if ( $.isPlainObject( rowSelector ) ) {
			// Indexes
			if ( rowSelector.row === undefined ) {
				// Selector options in first parameter
				opts = rowSelector;
				rowSelector = null;
			}
			else {
				// Cell index objects in first parameter
				opts = columnSelector;
				columnSelector = null;
			}
		}
		if ( $.isPlainObject( columnSelector ) ) {
			opts = columnSelector;
			columnSelector = null;
		}
	
		// Cell selector
		if ( columnSelector === null || columnSelector === undefined ) {
			return this.iterator( 'table', function ( settings ) {
				return __cell_selector( settings, rowSelector, _selector_opts( opts ) );
			} );
		}
	
		// Row + column selector
		var columns = this.columns( columnSelector, opts );
		var rows = this.rows( rowSelector, opts );
		var a, i, ien, j, jen;
	
		var cells = this.iterator( 'table', function ( settings, idx ) {
			a = [];
	
			for ( i=0, ien=rows[idx].length ; i<ien ; i++ ) {
				for ( j=0, jen=columns[idx].length ; j<jen ; j++ ) {
					a.push( {
						row:    rows[idx][i],
						column: columns[idx][j]
					} );
				}
			}
	
			return a;
		}, 1 );
	
		$.extend( cells.selector, {
			cols: columnSelector,
			rows: rowSelector,
			opts: opts
		} );
	
		return cells;
	} );
	
	
	_api_registerPlural( 'cells().nodes()', 'cell().node()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			var data = settings.aoData[ row ];
	
			return data && data.anCells ?
				data.anCells[ column ] :
				undefined;
		}, 1 );
	} );
	
	
	_api_register( 'cells().data()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return _fnGetCellData( settings, row, column );
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().cache()', 'cell().cache()', function ( type ) {
		type = type === 'search' ? '_aFilterData' : '_aSortData';
	
		return this.iterator( 'cell', function ( settings, row, column ) {
			return settings.aoData[ row ][ type ][ column ];
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().render()', 'cell().render()', function ( type ) {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return _fnGetCellData( settings, row, column, type );
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().indexes()', 'cell().index()', function () {
		return this.iterator( 'cell', function ( settings, row, column ) {
			return {
				row: row,
				column: column,
				columnVisible: _fnColumnIndexToVisible( settings, column )
			};
		}, 1 );
	} );
	
	
	_api_registerPlural( 'cells().invalidate()', 'cell().invalidate()', function ( src ) {
		return this.iterator( 'cell', function ( settings, row, column ) {
			_fnInvalidate( settings, row, src, column );
		} );
	} );
	
	
	
	_api_register( 'cell()', function ( rowSelector, columnSelector, opts ) {
		return _selector_first( this.cells( rowSelector, columnSelector, opts ) );
	} );
	
	
	_api_register( 'cell().data()', function ( data ) {
		var ctx = this.context;
		var cell = this[0];
	
		if ( data === undefined ) {
			// Get
			return ctx.length && cell.length ?
				_fnGetCellData( ctx[0], cell[0].row, cell[0].column ) :
				undefined;
		}
	
		// Set
		_fnSetCellData( ctx[0], cell[0].row, cell[0].column, data );
		_fnInvalidate( ctx[0], cell[0].row, 'data', cell[0].column );
	
		return this;
	} );
	
	
	
	/**
	 * Get current ordering (sorting) that has been applied to the table.
	 *
	 * @returns {array} 2D array containing the sorting information for the first
	 *   table in the current context. Each element in the parent array represents
	 *   a column being sorted upon (i.e. multi-sorting with two columns would have
	 *   2 inner arrays). The inner arrays may have 2 or 3 elements. The first is
	 *   the column index that the sorting condition applies to, the second is the
	 *   direction of the sort (`desc` or `asc`) and, optionally, the third is the
	 *   index of the sorting order from the `column.sorting` initialisation array.
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {integer} order Column index to sort upon.
	 * @param {string} direction Direction of the sort to be applied (`asc` or `desc`)
	 * @returns {DataTables.Api} this
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {array} order 1D array of sorting information to be applied.
	 * @param {array} [...] Optional additional sorting conditions
	 * @returns {DataTables.Api} this
	 *//**
	 * Set the ordering for the table.
	 *
	 * @param {array} order 2D array of sorting information to be applied.
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'order()', function ( order, dir ) {
		var ctx = this.context;
	
		if ( order === undefined ) {
			// get
			return ctx.length !== 0 ?
				ctx[0].aaSorting :
				undefined;
		}
	
		// set
		if ( typeof order === 'number' ) {
			// Simple column / direction passed in
			order = [ [ order, dir ] ];
		}
		else if ( order.length && ! $.isArray( order[0] ) ) {
			// Arguments passed in (list of 1D arrays)
			order = Array.prototype.slice.call( arguments );
		}
		// otherwise a 2D array was passed in
	
		return this.iterator( 'table', function ( settings ) {
			settings.aaSorting = order.slice();
		} );
	} );
	
	
	/**
	 * Attach a sort listener to an element for a given column
	 *
	 * @param {node|jQuery|string} node Identifier for the element(s) to attach the
	 *   listener to. This can take the form of a single DOM node, a jQuery
	 *   collection of nodes or a jQuery selector which will identify the node(s).
	 * @param {integer} column the column that a click on this node will sort on
	 * @param {function} [callback] callback function when sort is run
	 * @returns {DataTables.Api} this
	 */
	_api_register( 'order.listener()', function ( node, column, callback ) {
		return this.iterator( 'table', function ( settings ) {
			_fnSortAttachListener( settings, node, column, callback );
		} );
	} );
	
	
	_api_register( 'order.fixed()', function ( set ) {
		if ( ! set ) {
			var ctx = this.context;
			var fixed = ctx.length ?
				ctx[0].aaSortingFixed :
				undefined;
	
			return $.isArray( fixed ) ?
				{ pre: fixed } :
				fixed;
		}
	
		return this.iterator( 'table', function ( settings ) {
			settings.aaSortingFixed = $.extend( true, {}, set );
		} );
	} );
	
	
	// Order by the selected column(s)
	_api_register( [
		'columns().order()',
		'column().order()'
	], function ( dir ) {
		var that = this;
	
		return this.iterator( 'table', function ( settings, i ) {
			var sort = [];
	
			$.each( that[i], function (j, col) {
				sort.push( [ col, dir ] );
			} );
	
			settings.aaSorting = sort;
		} );
	} );
	
	
	
	_api_register( 'search()', function ( input, regex, smart, caseInsen ) {
		var ctx = this.context;
	
		if ( input === undefined ) {
			// get
			return ctx.length !== 0 ?
				ctx[0].oPreviousSearch.sSearch :
				undefined;
		}
	
		// set
		return this.iterator( 'table', function ( settings ) {
			if ( ! settings.oFeatures.bFilter ) {
				return;
			}
	
			_fnFilterComplete( settings, $.extend( {}, settings.oPreviousSearch, {
				"sSearch": input+"",
				"bRegex":  regex === null ? false : regex,
				"bSmart":  smart === null ? true  : smart,
				"bCaseInsensitive": caseInsen === null ? true : caseInsen
			} ), 1 );
		} );
	} );
	
	
	_api_registerPlural(
		'columns().search()',
		'column().search()',
		function ( input, regex, smart, caseInsen ) {
			return this.iterator( 'column', function ( settings, column ) {
				var preSearch = settings.aoPreSearchCols;
	
				if ( input === undefined ) {
					// get
					return preSearch[ column ].sSearch;
				}
	
				// set
				if ( ! settings.oFeatures.bFilter ) {
					return;
				}
	
				$.extend( preSearch[ column ], {
					"sSearch": input+"",
					"bRegex":  regex === null ? false : regex,
					"bSmart":  smart === null ? true  : smart,
					"bCaseInsensitive": caseInsen === null ? true : caseInsen
				} );
	
				_fnFilterComplete( settings, settings.oPreviousSearch, 1 );
			} );
		}
	);
	
	/*
	 * State API methods
	 */
	
	_api_register( 'state()', function () {
		return this.context.length ?
			this.context[0].oSavedState :
			null;
	} );
	
	
	_api_register( 'state.clear()', function () {
		return this.iterator( 'table', function ( settings ) {
			// Save an empty object
			settings.fnStateSaveCallback.call( settings.oInstance, settings, {} );
		} );
	} );
	
	
	_api_register( 'state.loaded()', function () {
		return this.context.length ?
			this.context[0].oLoadedState :
			null;
	} );
	
	
	_api_register( 'state.save()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnSaveState( settings );
		} );
	} );
	
	
	
	/**
	 * Provide a common method for plug-ins to check the version of DataTables being
	 * used, in order to ensure compatibility.
	 *
	 *  @param {string} version Version string to check for, in the format "X.Y.Z".
	 *    Note that the formats "X" and "X.Y" are also acceptable.
	 *  @returns {boolean} true if this version of DataTables is greater or equal to
	 *    the required version, or false if this version of DataTales is not
	 *    suitable
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    alert( $.fn.dataTable.versionCheck( '1.9.0' ) );
	 */
	DataTable.versionCheck = DataTable.fnVersionCheck = function( version )
	{
		var aThis = DataTable.version.split('.');
		var aThat = version.split('.');
		var iThis, iThat;
	
		for ( var i=0, iLen=aThat.length ; i<iLen ; i++ ) {
			iThis = parseInt( aThis[i], 10 ) || 0;
			iThat = parseInt( aThat[i], 10 ) || 0;
	
			// Parts are the same, keep comparing
			if (iThis === iThat) {
				continue;
			}
	
			// Parts are different, return immediately
			return iThis > iThat;
		}
	
		return true;
	};
	
	
	/**
	 * Check if a `<table>` node is a DataTable table already or not.
	 *
	 *  @param {node|jquery|string} table Table node, jQuery object or jQuery
	 *      selector for the table to test. Note that if more than more than one
	 *      table is passed on, only the first will be checked
	 *  @returns {boolean} true the table given is a DataTable, or false otherwise
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    if ( ! $.fn.DataTable.isDataTable( '#example' ) ) {
	 *      $('#example').dataTable();
	 *    }
	 */
	DataTable.isDataTable = DataTable.fnIsDataTable = function ( table )
	{
		var t = $(table).get(0);
		var is = false;
	
		if ( table instanceof DataTable.Api ) {
			return true;
		}
	
		$.each( DataTable.settings, function (i, o) {
			var head = o.nScrollHead ? $('table', o.nScrollHead)[0] : null;
			var foot = o.nScrollFoot ? $('table', o.nScrollFoot)[0] : null;
	
			if ( o.nTable === t || head === t || foot === t ) {
				is = true;
			}
		} );
	
		return is;
	};
	
	
	/**
	 * Get all DataTable tables that have been initialised - optionally you can
	 * select to get only currently visible tables.
	 *
	 *  @param {boolean} [visible=false] Flag to indicate if you want all (default)
	 *    or visible tables only.
	 *  @returns {array} Array of `table` nodes (not DataTable instances) which are
	 *    DataTables
	 *  @static
	 *  @dtopt API-Static
	 *
	 *  @example
	 *    $.each( $.fn.dataTable.tables(true), function () {
	 *      $(table).DataTable().columns.adjust();
	 *    } );
	 */
	DataTable.tables = DataTable.fnTables = function ( visible )
	{
		var api = false;
	
		if ( $.isPlainObject( visible ) ) {
			api = visible.api;
			visible = visible.visible;
		}
	
		var a = $.map( DataTable.settings, function (o) {
			if ( !visible || (visible && $(o.nTable).is(':visible')) ) {
				return o.nTable;
			}
		} );
	
		return api ?
			new _Api( a ) :
			a;
	};
	
	
	/**
	 * Convert from camel case parameters to Hungarian notation. This is made public
	 * for the extensions to provide the same ability as DataTables core to accept
	 * either the 1.9 style Hungarian notation, or the 1.10+ style camelCase
	 * parameters.
	 *
	 *  @param {object} src The model object which holds all parameters that can be
	 *    mapped.
	 *  @param {object} user The object to convert from camel case to Hungarian.
	 *  @param {boolean} force When set to `true`, properties which already have a
	 *    Hungarian value in the `user` object will be overwritten. Otherwise they
	 *    won't be.
	 */
	DataTable.camelToHungarian = _fnCamelToHungarian;
	
	
	
	/**
	 *
	 */
	_api_register( '$()', function ( selector, opts ) {
		var
			rows   = this.rows( opts ).nodes(), // Get all rows
			jqRows = $(rows);
	
		return $( [].concat(
			jqRows.filter( selector ).toArray(),
			jqRows.find( selector ).toArray()
		) );
	} );
	
	
	// jQuery functions to operate on the tables
	$.each( [ 'on', 'one', 'off' ], function (i, key) {
		_api_register( key+'()', function ( /* event, handler */ ) {
			var args = Array.prototype.slice.call(arguments);
	
			// Add the `dt` namespace automatically if it isn't already present
			args[0] = $.map( args[0].split( /\s/ ), function ( e ) {
				return ! e.match(/\.dt\b/) ?
					e+'.dt' :
					e;
				} ).join( ' ' );
	
			var inst = $( this.tables().nodes() );
			inst[key].apply( inst, args );
			return this;
		} );
	} );
	
	
	_api_register( 'clear()', function () {
		return this.iterator( 'table', function ( settings ) {
			_fnClearTable( settings );
		} );
	} );
	
	
	_api_register( 'settings()', function () {
		return new _Api( this.context, this.context );
	} );
	
	
	_api_register( 'init()', function () {
		var ctx = this.context;
		return ctx.length ? ctx[0].oInit : null;
	} );
	
	
	_api_register( 'data()', function () {
		return this.iterator( 'table', function ( settings ) {
			return _pluck( settings.aoData, '_aData' );
		} ).flatten();
	} );
	
	
	_api_register( 'destroy()', function ( remove ) {
		remove = remove || false;
	
		return this.iterator( 'table', function ( settings ) {
			var orig      = settings.nTableWrapper.parentNode;
			var classes   = settings.oClasses;
			var table     = settings.nTable;
			var tbody     = settings.nTBody;
			var thead     = settings.nTHead;
			var tfoot     = settings.nTFoot;
			var jqTable   = $(table);
			var jqTbody   = $(tbody);
			var jqWrapper = $(settings.nTableWrapper);
			var rows      = $.map( settings.aoData, function (r) { return r.nTr; } );
			var i, ien;
	
			// Flag to note that the table is currently being destroyed - no action
			// should be taken
			settings.bDestroying = true;
	
			// Fire off the destroy callbacks for plug-ins etc
			_fnCallbackFire( settings, "aoDestroyCallback", "destroy", [settings] );
	
			// If not being removed from the document, make all columns visible
			if ( ! remove ) {
				new _Api( settings ).columns().visible( true );
			}
	
			// Blitz all `DT` namespaced events (these are internal events, the
			// lowercase, `dt` events are user subscribed and they are responsible
			// for removing them
			jqWrapper.off('.DT').find(':not(tbody *)').off('.DT');
			$(window).off('.DT-'+settings.sInstance);
	
			// When scrolling we had to break the table up - restore it
			if ( table != thead.parentNode ) {
				jqTable.children('thead').detach();
				jqTable.append( thead );
			}
	
			if ( tfoot && table != tfoot.parentNode ) {
				jqTable.children('tfoot').detach();
				jqTable.append( tfoot );
			}
	
			settings.aaSorting = [];
			settings.aaSortingFixed = [];
			_fnSortingClasses( settings );
	
			$( rows ).removeClass( settings.asStripeClasses.join(' ') );
	
			$('th, td', thead).removeClass( classes.sSortable+' '+
				classes.sSortableAsc+' '+classes.sSortableDesc+' '+classes.sSortableNone
			);
	
			// Add the TR elements back into the table in their original order
			jqTbody.children().detach();
			jqTbody.append( rows );
	
			// Remove the DataTables generated nodes, events and classes
			var removedMethod = remove ? 'remove' : 'detach';
			jqTable[ removedMethod ]();
			jqWrapper[ removedMethod ]();
	
			// If we need to reattach the table to the document
			if ( ! remove && orig ) {
				// insertBefore acts like appendChild if !arg[1]
				orig.insertBefore( table, settings.nTableReinsertBefore );
	
				// Restore the width of the original table - was read from the style property,
				// so we can restore directly to that
				jqTable
					.css( 'width', settings.sDestroyWidth )
					.removeClass( classes.sTable );
	
				// If the were originally stripe classes - then we add them back here.
				// Note this is not fool proof (for example if not all rows had stripe
				// classes - but it's a good effort without getting carried away
				ien = settings.asDestroyStripes.length;
	
				if ( ien ) {
					jqTbody.children().each( function (i) {
						$(this).addClass( settings.asDestroyStripes[i % ien] );
					} );
				}
			}
	
			/* Remove the settings object from the settings array */
			var idx = $.inArray( settings, DataTable.settings );
			if ( idx !== -1 ) {
				DataTable.settings.splice( idx, 1 );
			}
		} );
	} );
	
	
	// Add the `every()` method for rows, columns and cells in a compact form
	$.each( [ 'column', 'row', 'cell' ], function ( i, type ) {
		_api_register( type+'s().every()', function ( fn ) {
			var opts = this.selector.opts;
			var api = this;
	
			return this.iterator( type, function ( settings, arg1, arg2, arg3, arg4 ) {
				// Rows and columns:
				//  arg1 - index
				//  arg2 - table counter
				//  arg3 - loop counter
				//  arg4 - undefined
				// Cells:
				//  arg1 - row index
				//  arg2 - column index
				//  arg3 - table counter
				//  arg4 - loop counter
				fn.call(
					api[ type ](
						arg1,
						type==='cell' ? arg2 : opts,
						type==='cell' ? opts : undefined
					),
					arg1, arg2, arg3, arg4
				);
			} );
		} );
	} );
	
	
	// i18n method for extensions to be able to use the language object from the
	// DataTable
	_api_register( 'i18n()', function ( token, def, plural ) {
		var ctx = this.context[0];
		var resolved = _fnGetObjectDataFn( token )( ctx.oLanguage );
	
		if ( resolved === undefined ) {
			resolved = def;
		}
	
		if ( plural !== undefined && $.isPlainObject( resolved ) ) {
			resolved = resolved[ plural ] !== undefined ?
				resolved[ plural ] :
				resolved._;
		}
	
		return resolved.replace( '%d', plural ); // nb: plural might be undefined,
	} );

	/**
	 * Version string for plug-ins to check compatibility. Allowed format is
	 * `a.b.c-d` where: a:int, b:int, c:int, d:string(dev|beta|alpha). `d` is used
	 * only for non-release builds. See http://semver.org/ for more information.
	 *  @member
	 *  @type string
	 *  @default Version number
	 */
	DataTable.version = "1.10.16";

	/**
	 * Private data store, containing all of the settings objects that are
	 * created for the tables on a given page.
	 *
	 * Note that the `DataTable.settings` object is aliased to
	 * `jQuery.fn.dataTableExt` through which it may be accessed and
	 * manipulated, or `jQuery.fn.dataTable.settings`.
	 *  @member
	 *  @type array
	 *  @default []
	 *  @private
	 */
	DataTable.settings = [];

	/**
	 * Object models container, for the various models that DataTables has
	 * available to it. These models define the objects that are used to hold
	 * the active state and configuration of the table.
	 *  @namespace
	 */
	DataTable.models = {};
	
	
	
	/**
	 * Template object for the way in which DataTables holds information about
	 * search information for the global filter and individual column filters.
	 *  @namespace
	 */
	DataTable.models.oSearch = {
		/**
		 * Flag to indicate if the filtering should be case insensitive or not
		 *  @type boolean
		 *  @default true
		 */
		"bCaseInsensitive": true,
	
		/**
		 * Applied search term
		 *  @type string
		 *  @default <i>Empty string</i>
		 */
		"sSearch": "",
	
		/**
		 * Flag to indicate if the search term should be interpreted as a
		 * regular expression (true) or not (false) and therefore and special
		 * regex characters escaped.
		 *  @type boolean
		 *  @default false
		 */
		"bRegex": false,
	
		/**
		 * Flag to indicate if DataTables is to use its smart filtering or not.
		 *  @type boolean
		 *  @default true
		 */
		"bSmart": true
	};
	
	
	
	
	/**
	 * Template object for the way in which DataTables holds information about
	 * each individual row. This is the object format used for the settings
	 * aoData array.
	 *  @namespace
	 */
	DataTable.models.oRow = {
		/**
		 * TR element for the row
		 *  @type node
		 *  @default null
		 */
		"nTr": null,
	
		/**
		 * Array of TD elements for each row. This is null until the row has been
		 * created.
		 *  @type array nodes
		 *  @default []
		 */
		"anCells": null,
	
		/**
		 * Data object from the original data source for the row. This is either
		 * an array if using the traditional form of DataTables, or an object if
		 * using mData options. The exact type will depend on the passed in
		 * data from the data source, or will be an array if using DOM a data
		 * source.
		 *  @type array|object
		 *  @default []
		 */
		"_aData": [],
	
		/**
		 * Sorting data cache - this array is ostensibly the same length as the
		 * number of columns (although each index is generated only as it is
		 * needed), and holds the data that is used for sorting each column in the
		 * row. We do this cache generation at the start of the sort in order that
		 * the formatting of the sort data need be done only once for each cell
		 * per sort. This array should not be read from or written to by anything
		 * other than the master sorting methods.
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_aSortData": null,
	
		/**
		 * Per cell filtering data cache. As per the sort data cache, used to
		 * increase the performance of the filtering in DataTables
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_aFilterData": null,
	
		/**
		 * Filtering data cache. This is the same as the cell filtering cache, but
		 * in this case a string rather than an array. This is easily computed with
		 * a join on `_aFilterData`, but is provided as a cache so the join isn't
		 * needed on every search (memory traded for performance)
		 *  @type array
		 *  @default null
		 *  @private
		 */
		"_sFilterRow": null,
	
		/**
		 * Cache of the class name that DataTables has applied to the row, so we
		 * can quickly look at this variable rather than needing to do a DOM check
		 * on className for the nTr property.
		 *  @type string
		 *  @default <i>Empty string</i>
		 *  @private
		 */
		"_sRowStripe": "",
	
		/**
		 * Denote if the original data source was from the DOM, or the data source
		 * object. This is used for invalidating data, so DataTables can
		 * automatically read data from the original source, unless uninstructed
		 * otherwise.
		 *  @type string
		 *  @default null
		 *  @private
		 */
		"src": null,
	
		/**
		 * Index in the aoData array. This saves an indexOf lookup when we have the
		 * object, but want to know the index
		 *  @type integer
		 *  @default -1
		 *  @private
		 */
		"idx": -1
	};
	
	
	/**
	 * Template object for the column information object in DataTables. This object
	 * is held in the settings aoColumns array and contains all the information that
	 * DataTables needs about each individual column.
	 *
	 * Note that this object is related to {@link DataTable.defaults.column}
	 * but this one is the internal data store for DataTables's cache of columns.
	 * It should NOT be manipulated outside of DataTables. Any configuration should
	 * be done through the initialisation options.
	 *  @namespace
	 */
	DataTable.models.oColumn = {
		/**
		 * Column index. This could be worked out on-the-fly with $.inArray, but it
		 * is faster to just hold it as a variable
		 *  @type integer
		 *  @default null
		 */
		"idx": null,
	
		/**
		 * A list of the columns that sorting should occur on when this column
		 * is sorted. That this property is an array allows multi-column sorting
		 * to be defined for a column (for example first name / last name columns
		 * would benefit from this). The values are integers pointing to the
		 * columns to be sorted on (typically it will be a single integer pointing
		 * at itself, but that doesn't need to be the case).
		 *  @type array
		 */
		"aDataSort": null,
	
		/**
		 * Define the sorting directions that are applied to the column, in sequence
		 * as the column is repeatedly sorted upon - i.e. the first value is used
		 * as the sorting direction when the column if first sorted (clicked on).
		 * Sort it again (click again) and it will move on to the next index.
		 * Repeat until loop.
		 *  @type array
		 */
		"asSorting": null,
	
		/**
		 * Flag to indicate if the column is searchable, and thus should be included
		 * in the filtering or not.
		 *  @type boolean
		 */
		"bSearchable": null,
	
		/**
		 * Flag to indicate if the column is sortable or not.
		 *  @type boolean
		 */
		"bSortable": null,
	
		/**
		 * Flag to indicate if the column is currently visible in the table or not
		 *  @type boolean
		 */
		"bVisible": null,
	
		/**
		 * Store for manual type assignment using the `column.type` option. This
		 * is held in store so we can manipulate the column's `sType` property.
		 *  @type string
		 *  @default null
		 *  @private
		 */
		"_sManualType": null,
	
		/**
		 * Flag to indicate if HTML5 data attributes should be used as the data
		 * source for filtering or sorting. True is either are.
		 *  @type boolean
		 *  @default false
		 *  @private
		 */
		"_bAttrSrc": false,
	
		/**
		 * Developer definable function that is called whenever a cell is created (Ajax source,
		 * etc) or processed for input (DOM source). This can be used as a compliment to mRender
		 * allowing you to modify the DOM element (add background colour for example) when the
		 * element is available.
		 *  @type function
		 *  @param {element} nTd The TD node that has been created
		 *  @param {*} sData The Data for the cell
		 *  @param {array|object} oData The data for the whole row
		 *  @param {int} iRow The row index for the aoData data store
		 *  @default null
		 */
		"fnCreatedCell": null,
	
		/**
		 * Function to get data from a cell in a column. You should <b>never</b>
		 * access data directly through _aData internally in DataTables - always use
		 * the method attached to this property. It allows mData to function as
		 * required. This function is automatically assigned by the column
		 * initialisation method
		 *  @type function
		 *  @param {array|object} oData The data array/object for the array
		 *    (i.e. aoData[]._aData)
		 *  @param {string} sSpecific The specific data type you want to get -
		 *    'display', 'type' 'filter' 'sort'
		 *  @returns {*} The data for the cell from the given row's data
		 *  @default null
		 */
		"fnGetData": null,
	
		/**
		 * Function to set data for a cell in the column. You should <b>never</b>
		 * set the data directly to _aData internally in DataTables - always use
		 * this method. It allows mData to function as required. This function
		 * is automatically assigned by the column initialisation method
		 *  @type function
		 *  @param {array|object} oData The data array/object for the array
		 *    (i.e. aoData[]._aData)
		 *  @param {*} sValue Value to set
		 *  @default null
		 */
		"fnSetData": null,
	
		/**
		 * Property to read the value for the cells in the column from the data
		 * source array / object. If null, then the default content is used, if a
		 * function is given then the return from the function is used.
		 *  @type function|int|string|null
		 *  @default null
		 */
		"mData": null,
	
		/**
		 * Partner property to mData which is used (only when defined) to get
		 * the data - i.e. it is basically the same as mData, but without the
		 * 'set' option, and also the data fed to it is the result from mData.
		 * This is the rendering method to match the data method of mData.
		 *  @type function|int|string|null
		 *  @default null
		 */
		"mRender": null,
	
		/**
		 * Unique header TH/TD element for this column - this is what the sorting
		 * listener is attached to (if sorting is enabled.)
		 *  @type node
		 *  @default null
		 */
		"nTh": null,
	
		/**
		 * Unique footer TH/TD element for this column (if there is one). Not used
		 * in DataTables as such, but can be used for plug-ins to reference the
		 * footer for each column.
		 *  @type node
		 *  @default null
		 */
		"nTf": null,
	
		/**
		 * The class to apply to all TD elements in the table's TBODY for the column
		 *  @type string
		 *  @default null
		 */
		"sClass": null,
	
		/**
		 * When DataTables calculates the column widths to assign to each column,
		 * it finds the longest string in each column and then constructs a
		 * temporary table and reads the widths from that. The problem with this
		 * is that "mmm" is much wider then "iiii", but the latter is a longer
		 * string - thus the calculation can go wrong (doing it properly and putting
		 * it into an DOM object and measuring that is horribly(!) slow). Thus as
		 * a "work around" we provide this option. It will append its value to the
		 * text that is found to be the longest string for the column - i.e. padding.
		 *  @type string
		 */
		"sContentPadding": null,
	
		/**
		 * Allows a default value to be given for a column's data, and will be used
		 * whenever a null data source is encountered (this can be because mData
		 * is set to null, or because the data source itself is null).
		 *  @type string
		 *  @default null
		 */
		"sDefaultContent": null,
	
		/**
		 * Name for the column, allowing reference to the column by name as well as
		 * by index (needs a lookup to work by name).
		 *  @type string
		 */
		"sName": null,
	
		/**
		 * Custom sorting data type - defines which of the available plug-ins in
		 * afnSortData the custom sorting will use - if any is defined.
		 *  @type string
		 *  @default std
		 */
		"sSortDataType": 'std',
	
		/**
		 * Class to be applied to the header element when sorting on this column
		 *  @type string
		 *  @default null
		 */
		"sSortingClass": null,
	
		/**
		 * Class to be applied to the header element when sorting on this column -
		 * when jQuery UI theming is used.
		 *  @type string
		 *  @default null
		 */
		"sSortingClassJUI": null,
	
		/**
		 * Title of the column - what is seen in the TH element (nTh).
		 *  @type string
		 */
		"sTitle": null,
	
		/**
		 * Column sorting and filtering type
		 *  @type string
		 *  @default null
		 */
		"sType": null,
	
		/**
		 * Width of the column
		 *  @type string
		 *  @default null
		 */
		"sWidth": null,
	
		/**
		 * Width of the column when it was first "encountered"
		 *  @type string
		 *  @default null
		 */
		"sWidthOrig": null
	};
	
	
	/*
	 * Developer note: The properties of the object below are given in Hungarian
	 * notation, that was used as the interface for DataTables prior to v1.10, however
	 * from v1.10 onwards the primary interface is camel case. In order to avoid
	 * breaking backwards compatibility utterly with this change, the Hungarian
	 * version is still, internally the primary interface, but is is not documented
	 * - hence the @name tags in each doc comment. This allows a Javascript function
	 * to create a map from Hungarian notation to camel case (going the other direction
	 * would require each property to be listed, which would at around 3K to the size
	 * of DataTables, while this method is about a 0.5K hit.
	 *
	 * Ultimately this does pave the way for Hungarian notation to be dropped
	 * completely, but that is a massive amount of work and will break current
	 * installs (therefore is on-hold until v2).
	 */
	
	/**
	 * Initialisation options that can be given to DataTables at initialisation
	 * time.
	 *  @namespace
	 */
	DataTable.defaults = {
		/**
		 * An array of data to use for the table, passed in at initialisation which
		 * will be used in preference to any data which is already in the DOM. This is
		 * particularly useful for constructing tables purely in Javascript, for
		 * example with a custom Ajax call.
		 *  @type array
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.data
		 *
		 *  @example
		 *    // Using a 2D array data source
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "data": [
		 *          ['Trident', 'Internet Explorer 4.0', 'Win 95+', 4, 'X'],
		 *          ['Trident', 'Internet Explorer 5.0', 'Win 95+', 5, 'C'],
		 *        ],
		 *        "columns": [
		 *          { "title": "Engine" },
		 *          { "title": "Browser" },
		 *          { "title": "Platform" },
		 *          { "title": "Version" },
		 *          { "title": "Grade" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using an array of objects as a data source (`data`)
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "data": [
		 *          {
		 *            "engine":   "Trident",
		 *            "browser":  "Internet Explorer 4.0",
		 *            "platform": "Win 95+",
		 *            "version":  4,
		 *            "grade":    "X"
		 *          },
		 *          {
		 *            "engine":   "Trident",
		 *            "browser":  "Internet Explorer 5.0",
		 *            "platform": "Win 95+",
		 *            "version":  5,
		 *            "grade":    "C"
		 *          }
		 *        ],
		 *        "columns": [
		 *          { "title": "Engine",   "data": "engine" },
		 *          { "title": "Browser",  "data": "browser" },
		 *          { "title": "Platform", "data": "platform" },
		 *          { "title": "Version",  "data": "version" },
		 *          { "title": "Grade",    "data": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"aaData": null,
	
	
		/**
		 * If ordering is enabled, then DataTables will perform a first pass sort on
		 * initialisation. You can define which column(s) the sort is performed
		 * upon, and the sorting direction, with this variable. The `sorting` array
		 * should contain an array for each column to be sorted initially containing
		 * the column's index and a direction string ('asc' or 'desc').
		 *  @type array
		 *  @default [[0,'asc']]
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.order
		 *
		 *  @example
		 *    // Sort by 3rd column first, and then 4th column
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "order": [[2,'asc'], [3,'desc']]
		 *      } );
		 *    } );
		 *
		 *    // No initial sorting
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "order": []
		 *      } );
		 *    } );
		 */
		"aaSorting": [[0,'asc']],
	
	
		/**
		 * This parameter is basically identical to the `sorting` parameter, but
		 * cannot be overridden by user interaction with the table. What this means
		 * is that you could have a column (visible or hidden) which the sorting
		 * will always be forced on first - any sorting after that (from the user)
		 * will then be performed as required. This can be useful for grouping rows
		 * together.
		 *  @type array
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.orderFixed
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "orderFixed": [[0,'asc']]
		 *      } );
		 *    } )
		 */
		"aaSortingFixed": [],
	
	
		/**
		 * DataTables can be instructed to load data to display in the table from a
		 * Ajax source. This option defines how that Ajax call is made and where to.
		 *
		 * The `ajax` property has three different modes of operation, depending on
		 * how it is defined. These are:
		 *
		 * * `string` - Set the URL from where the data should be loaded from.
		 * * `object` - Define properties for `jQuery.ajax`.
		 * * `function` - Custom data get function
		 *
		 * `string`
		 * --------
		 *
		 * As a string, the `ajax` property simply defines the URL from which
		 * DataTables will load data.
		 *
		 * `object`
		 * --------
		 *
		 * As an object, the parameters in the object are passed to
		 * [jQuery.ajax](http://api.jquery.com/jQuery.ajax/) allowing fine control
		 * of the Ajax request. DataTables has a number of default parameters which
		 * you can override using this option. Please refer to the jQuery
		 * documentation for a full description of the options available, although
		 * the following parameters provide additional options in DataTables or
		 * require special consideration:
		 *
		 * * `data` - As with jQuery, `data` can be provided as an object, but it
		 *   can also be used as a function to manipulate the data DataTables sends
		 *   to the server. The function takes a single parameter, an object of
		 *   parameters with the values that DataTables has readied for sending. An
		 *   object may be returned which will be merged into the DataTables
		 *   defaults, or you can add the items to the object that was passed in and
		 *   not return anything from the function. This supersedes `fnServerParams`
		 *   from DataTables 1.9-.
		 *
		 * * `dataSrc` - By default DataTables will look for the property `data` (or
		 *   `aaData` for compatibility with DataTables 1.9-) when obtaining data
		 *   from an Ajax source or for server-side processing - this parameter
		 *   allows that property to be changed. You can use Javascript dotted
		 *   object notation to get a data source for multiple levels of nesting, or
		 *   it my be used as a function. As a function it takes a single parameter,
		 *   the JSON returned from the server, which can be manipulated as
		 *   required, with the returned value being that used by DataTables as the
		 *   data source for the table. This supersedes `sAjaxDataProp` from
		 *   DataTables 1.9-.
		 *
		 * * `success` - Should not be overridden it is used internally in
		 *   DataTables. To manipulate / transform the data returned by the server
		 *   use `ajax.dataSrc`, or use `ajax` as a function (see below).
		 *
		 * `function`
		 * ----------
		 *
		 * As a function, making the Ajax call is left up to yourself allowing
		 * complete control of the Ajax request. Indeed, if desired, a method other
		 * than Ajax could be used to obtain the required data, such as Web storage
		 * or an AIR database.
		 *
		 * The function is given four parameters and no return is required. The
		 * parameters are:
		 *
		 * 1. _object_ - Data to send to the server
		 * 2. _function_ - Callback function that must be executed when the required
		 *    data has been obtained. That data should be passed into the callback
		 *    as the only parameter
		 * 3. _object_ - DataTables settings object for the table
		 *
		 * Note that this supersedes `fnServerData` from DataTables 1.9-.
		 *
		 *  @type string|object|function
		 *  @default null
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.ajax
		 *  @since 1.10.0
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax.
		 *   // Note DataTables expects data in the form `{ data: [ ...data... ] }` by default).
		 *   $('#example').dataTable( {
		 *     "ajax": "data.json"
		 *   } );
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax, using `dataSrc` to change
		 *   // `data` to `tableData` (i.e. `{ tableData: [ ...data... ] }`)
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": "tableData"
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Get JSON data from a file via Ajax, using `dataSrc` to read data
		 *   // from a plain array rather than an array in an object
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": ""
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Manipulate the data returned from the server - add a link to data
		 *   // (note this can, should, be done using `render` for the column - this
		 *   // is just a simple example of how the data can be manipulated).
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "dataSrc": function ( json ) {
		 *         for ( var i=0, ien=json.length ; i<ien ; i++ ) {
		 *           json[i][0] = '<a href="/message/'+json[i][0]+'>View message</a>';
		 *         }
		 *         return json;
		 *       }
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Add data to the request
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "data": function ( d ) {
		 *         return {
		 *           "extra_search": $('#extra').val()
		 *         };
		 *       }
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Send request as POST
		 *   $('#example').dataTable( {
		 *     "ajax": {
		 *       "url": "data.json",
		 *       "type": "POST"
		 *     }
		 *   } );
		 *
		 * @example
		 *   // Get the data from localStorage (could interface with a form for
		 *   // adding, editing and removing rows).
		 *   $('#example').dataTable( {
		 *     "ajax": function (data, callback, settings) {
		 *       callback(
		 *         JSON.parse( localStorage.getItem('dataTablesData') )
		 *       );
		 *     }
		 *   } );
		 */
		"ajax": null,
	
	
		/**
		 * This parameter allows you to readily specify the entries in the length drop
		 * down menu that DataTables shows when pagination is enabled. It can be
		 * either a 1D array of options which will be used for both the displayed
		 * option and the value, or a 2D array which will use the array in the first
		 * position as the value, and the array in the second position as the
		 * displayed options (useful for language strings such as 'All').
		 *
		 * Note that the `pageLength` property will be automatically set to the
		 * first value given in this array, unless `pageLength` is also provided.
		 *  @type array
		 *  @default [ 10, 25, 50, 100 ]
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.lengthMenu
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
		 *      } );
		 *    } );
		 */
		"aLengthMenu": [ 10, 25, 50, 100 ],
	
	
		/**
		 * The `columns` option in the initialisation parameter allows you to define
		 * details about the way individual columns behave. For a full list of
		 * column options that can be set, please see
		 * {@link DataTable.defaults.column}. Note that if you use `columns` to
		 * define your columns, you must have an entry in the array for every single
		 * column that you have in your table (these can be null if you don't which
		 * to specify any options).
		 *  @member
		 *
		 *  @name DataTable.defaults.column
		 */
		"aoColumns": null,
	
		/**
		 * Very similar to `columns`, `columnDefs` allows you to target a specific
		 * column, multiple columns, or all columns, using the `targets` property of
		 * each object in the array. This allows great flexibility when creating
		 * tables, as the `columnDefs` arrays can be of any length, targeting the
		 * columns you specifically want. `columnDefs` may use any of the column
		 * options available: {@link DataTable.defaults.column}, but it _must_
		 * have `targets` defined in each object in the array. Values in the `targets`
		 * array may be:
		 *   <ul>
		 *     <li>a string - class name will be matched on the TH for the column</li>
		 *     <li>0 or a positive integer - column index counting from the left</li>
		 *     <li>a negative integer - column index counting from the right</li>
		 *     <li>the string "_all" - all columns (i.e. assign a default)</li>
		 *   </ul>
		 *  @member
		 *
		 *  @name DataTable.defaults.columnDefs
		 */
		"aoColumnDefs": null,
	
	
		/**
		 * Basically the same as `search`, this parameter defines the individual column
		 * filtering state at initialisation time. The array must be of the same size
		 * as the number of columns, and each element be an object with the parameters
		 * `search` and `escapeRegex` (the latter is optional). 'null' is also
		 * accepted and the default will be used.
		 *  @type array
		 *  @default []
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.searchCols
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "searchCols": [
		 *          null,
		 *          { "search": "My filter" },
		 *          null,
		 *          { "search": "^[0-9]", "escapeRegex": false }
		 *        ]
		 *      } );
		 *    } )
		 */
		"aoSearchCols": [],
	
	
		/**
		 * An array of CSS classes that should be applied to displayed rows. This
		 * array may be of any length, and DataTables will apply each class
		 * sequentially, looping when required.
		 *  @type array
		 *  @default null <i>Will take the values determined by the `oClasses.stripe*`
		 *    options</i>
		 *
		 *  @dtopt Option
		 *  @name DataTable.defaults.stripeClasses
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stripeClasses": [ 'strip1', 'strip2', 'strip3' ]
		 *      } );
		 *    } )
		 */
		"asStripeClasses": null,
	
	
		/**
		 * Enable or disable automatic column width calculation. This can be disabled
		 * as an optimisation (it takes some time to calculate the widths) if the
		 * tables widths are passed in using `columns`.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.autoWidth
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "autoWidth": false
		 *      } );
		 *    } );
		 */
		"bAutoWidth": true,
	
	
		/**
		 * Deferred rendering can provide DataTables with a huge speed boost when you
		 * are using an Ajax or JS data source for the table. This option, when set to
		 * true, will cause DataTables to defer the creation of the table elements for
		 * each row until they are needed for a draw - saving a significant amount of
		 * time.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.deferRender
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajax": "sources/arrays.txt",
		 *        "deferRender": true
		 *      } );
		 *    } );
		 */
		"bDeferRender": false,
	
	
		/**
		 * Replace a DataTable which matches the given selector and replace it with
		 * one which has the properties of the new initialisation object passed. If no
		 * table matches the selector, then the new DataTable will be constructed as
		 * per normal.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.destroy
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "srollY": "200px",
		 *        "paginate": false
		 *      } );
		 *
		 *      // Some time later....
		 *      $('#example').dataTable( {
		 *        "filter": false,
		 *        "destroy": true
		 *      } );
		 *    } );
		 */
		"bDestroy": false,
	
	
		/**
		 * Enable or disable filtering of data. Filtering in DataTables is "smart" in
		 * that it allows the end user to input multiple words (space separated) and
		 * will match a row containing those words, even if not in the order that was
		 * specified (this allow matching across multiple columns). Note that if you
		 * wish to use filtering in DataTables this must remain 'true' - to remove the
		 * default filtering input box and retain filtering abilities, please use
		 * {@link DataTable.defaults.dom}.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.searching
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "searching": false
		 *      } );
		 *    } );
		 */
		"bFilter": true,
	
	
		/**
		 * Enable or disable the table information display. This shows information
		 * about the data that is currently visible on the page, including information
		 * about filtered data if that action is being performed.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.info
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "info": false
		 *      } );
		 *    } );
		 */
		"bInfo": true,
	
	
		/**
		 * Allows the end user to select the size of a formatted page from a select
		 * menu (sizes are 10, 25, 50 and 100). Requires pagination (`paginate`).
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.lengthChange
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "lengthChange": false
		 *      } );
		 *    } );
		 */
		"bLengthChange": true,
	
	
		/**
		 * Enable or disable pagination.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.paging
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "paging": false
		 *      } );
		 *    } );
		 */
		"bPaginate": true,
	
	
		/**
		 * Enable or disable the display of a 'processing' indicator when the table is
		 * being processed (e.g. a sort). This is particularly useful for tables with
		 * large amounts of data where it can take a noticeable amount of time to sort
		 * the entries.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.processing
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "processing": true
		 *      } );
		 *    } );
		 */
		"bProcessing": false,
	
	
		/**
		 * Retrieve the DataTables object for the given selector. Note that if the
		 * table has already been initialised, this parameter will cause DataTables
		 * to simply return the object that has already been set up - it will not take
		 * account of any changes you might have made to the initialisation object
		 * passed to DataTables (setting this parameter to true is an acknowledgement
		 * that you understand this). `destroy` can be used to reinitialise a table if
		 * you need.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.retrieve
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      initTable();
		 *      tableActions();
		 *    } );
		 *
		 *    function initTable ()
		 *    {
		 *      return $('#example').dataTable( {
		 *        "scrollY": "200px",
		 *        "paginate": false,
		 *        "retrieve": true
		 *      } );
		 *    }
		 *
		 *    function tableActions ()
		 *    {
		 *      var table = initTable();
		 *      // perform API operations with oTable
		 *    }
		 */
		"bRetrieve": false,
	
	
		/**
		 * When vertical (y) scrolling is enabled, DataTables will force the height of
		 * the table's viewport to the given height at all times (useful for layout).
		 * However, this can look odd when filtering data down to a small data set,
		 * and the footer is left "floating" further down. This parameter (when
		 * enabled) will cause DataTables to collapse the table's viewport down when
		 * the result set will fit within the given Y height.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.scrollCollapse
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollY": "200",
		 *        "scrollCollapse": true
		 *      } );
		 *    } );
		 */
		"bScrollCollapse": false,
	
	
		/**
		 * Configure DataTables to use server-side processing. Note that the
		 * `ajax` parameter must also be given in order to give DataTables a
		 * source to obtain the required data for each draw.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverSide
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "xhr.php"
		 *      } );
		 *    } );
		 */
		"bServerSide": false,
	
	
		/**
		 * Enable or disable sorting of columns. Sorting of individual columns can be
		 * disabled by the `sortable` option for each column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.ordering
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "ordering": false
		 *      } );
		 *    } );
		 */
		"bSort": true,
	
	
		/**
		 * Enable or display DataTables' ability to sort multiple columns at the
		 * same time (activated by shift-click by the user).
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.orderMulti
		 *
		 *  @example
		 *    // Disable multiple column sorting ability
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "orderMulti": false
		 *      } );
		 *    } );
		 */
		"bSortMulti": true,
	
	
		/**
		 * Allows control over whether DataTables should use the top (true) unique
		 * cell that is found for a single column, or the bottom (false - default).
		 * This is useful when using complex headers.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.orderCellsTop
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "orderCellsTop": true
		 *      } );
		 *    } );
		 */
		"bSortCellsTop": false,
	
	
		/**
		 * Enable or disable the addition of the classes `sorting\_1`, `sorting\_2` and
		 * `sorting\_3` to the columns which are currently being sorted on. This is
		 * presented as a feature switch as it can increase processing time (while
		 * classes are removed and added) so for large data sets you might want to
		 * turn this off.
		 *  @type boolean
		 *  @default true
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.orderClasses
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "orderClasses": false
		 *      } );
		 *    } );
		 */
		"bSortClasses": true,
	
	
		/**
		 * Enable or disable state saving. When enabled HTML5 `localStorage` will be
		 * used to save table display information such as pagination information,
		 * display length, filtering and sorting. As such when the end user reloads
		 * the page the display display will match what thy had previously set up.
		 *
		 * Due to the use of `localStorage` the default state saving is not supported
		 * in IE6 or 7. If state saving is required in those browsers, use
		 * `stateSaveCallback` to provide a storage solution such as cookies.
		 *  @type boolean
		 *  @default false
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.stateSave
		 *
		 *  @example
		 *    $(document).ready( function () {
		 *      $('#example').dataTable( {
		 *        "stateSave": true
		 *      } );
		 *    } );
		 */
		"bStateSave": false,
	
	
		/**
		 * This function is called when a TR element is created (and all TD child
		 * elements have been inserted), or registered if using a DOM source, allowing
		 * manipulation of the TR element (adding classes etc).
		 *  @type function
		 *  @param {node} row "TR" element for the current row
		 *  @param {array} data Raw data array for this row
		 *  @param {int} dataIndex The index of this row in the internal aoData array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.createdRow
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "createdRow": function( row, data, dataIndex ) {
		 *          // Bold the grade for all 'A' grade browsers
		 *          if ( data[4] == "A" )
		 *          {
		 *            $('td:eq(4)', row).html( '<b>A</b>' );
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnCreatedRow": null,
	
	
		/**
		 * This function is called on every 'draw' event, and allows you to
		 * dynamically modify any aspect you want about the created DOM.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.drawCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "drawCallback": function( settings ) {
		 *          alert( 'DataTables has redrawn the table' );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnDrawCallback": null,
	
	
		/**
		 * Identical to fnHeaderCallback() but for the table footer this function
		 * allows you to modify the table footer on every 'draw' event.
		 *  @type function
		 *  @param {node} foot "TR" element for the footer
		 *  @param {array} data Full table data (as derived from the original HTML)
		 *  @param {int} start Index for the current display starting point in the
		 *    display array
		 *  @param {int} end Index for the current display ending point in the
		 *    display array
		 *  @param {array int} display Index array to translate the visual position
		 *    to the full data array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.footerCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "footerCallback": function( tfoot, data, start, end, display ) {
		 *          tfoot.getElementsByTagName('th')[0].innerHTML = "Starting index is "+start;
		 *        }
		 *      } );
		 *    } )
		 */
		"fnFooterCallback": null,
	
	
		/**
		 * When rendering large numbers in the information element for the table
		 * (i.e. "Showing 1 to 10 of 57 entries") DataTables will render large numbers
		 * to have a comma separator for the 'thousands' units (e.g. 1 million is
		 * rendered as "1,000,000") to help readability for the end user. This
		 * function will override the default method DataTables uses.
		 *  @type function
		 *  @member
		 *  @param {int} toFormat number to be formatted
		 *  @returns {string} formatted string for DataTables to show the number
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.formatNumber
		 *
		 *  @example
		 *    // Format a number using a single quote for the separator (note that
		 *    // this can also be done with the language.thousands option)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "formatNumber": function ( toFormat ) {
		 *          return toFormat.toString().replace(
		 *            /\B(?=(\d{3})+(?!\d))/g, "'"
		 *          );
		 *        };
		 *      } );
		 *    } );
		 */
		"fnFormatNumber": function ( toFormat ) {
			return toFormat.toString().replace(
				/\B(?=(\d{3})+(?!\d))/g,
				this.oLanguage.sThousands
			);
		},
	
	
		/**
		 * This function is called on every 'draw' event, and allows you to
		 * dynamically modify the header row. This can be used to calculate and
		 * display useful information about the table.
		 *  @type function
		 *  @param {node} head "TR" element for the header
		 *  @param {array} data Full table data (as derived from the original HTML)
		 *  @param {int} start Index for the current display starting point in the
		 *    display array
		 *  @param {int} end Index for the current display ending point in the
		 *    display array
		 *  @param {array int} display Index array to translate the visual position
		 *    to the full data array
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.headerCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "fheaderCallback": function( head, data, start, end, display ) {
		 *          head.getElementsByTagName('th')[0].innerHTML = "Displaying "+(end-start)+" records";
		 *        }
		 *      } );
		 *    } )
		 */
		"fnHeaderCallback": null,
	
	
		/**
		 * The information element can be used to convey information about the current
		 * state of the table. Although the internationalisation options presented by
		 * DataTables are quite capable of dealing with most customisations, there may
		 * be times where you wish to customise the string further. This callback
		 * allows you to do exactly that.
		 *  @type function
		 *  @param {object} oSettings DataTables settings object
		 *  @param {int} start Starting position in data for the draw
		 *  @param {int} end End position in data for the draw
		 *  @param {int} max Total number of rows in the table (regardless of
		 *    filtering)
		 *  @param {int} total Total number of rows in the data set, after filtering
		 *  @param {string} pre The string that DataTables has formatted using it's
		 *    own rules
		 *  @returns {string} The string to be displayed in the information element.
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.infoCallback
		 *
		 *  @example
		 *    $('#example').dataTable( {
		 *      "infoCallback": function( settings, start, end, max, total, pre ) {
		 *        return start +" to "+ end;
		 *      }
		 *    } );
		 */
		"fnInfoCallback": null,
	
	
		/**
		 * Called when the table has been initialised. Normally DataTables will
		 * initialise sequentially and there will be no need for this function,
		 * however, this does not hold true when using external language information
		 * since that is obtained using an async XHR call.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} json The JSON object request from the server - only
		 *    present if client-side Ajax sourced data is used
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.initComplete
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "initComplete": function(settings, json) {
		 *          alert( 'DataTables has finished its initialisation.' );
		 *        }
		 *      } );
		 *    } )
		 */
		"fnInitComplete": null,
	
	
		/**
		 * Called at the very start of each table draw and can be used to cancel the
		 * draw by returning false, any other return (including undefined) results in
		 * the full draw occurring).
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @returns {boolean} False will cancel the draw, anything else (including no
		 *    return) will allow it to complete.
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.preDrawCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "preDrawCallback": function( settings ) {
		 *          if ( $('#test').val() == 1 ) {
		 *            return false;
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnPreDrawCallback": null,
	
	
		/**
		 * This function allows you to 'post process' each row after it have been
		 * generated for each table draw, but before it is rendered on screen. This
		 * function might be used for setting the row class name etc.
		 *  @type function
		 *  @param {node} row "TR" element for the current row
		 *  @param {array} data Raw data array for this row
		 *  @param {int} displayIndex The display index for the current table draw
		 *  @param {int} displayIndexFull The index of the data in the full list of
		 *    rows (after filtering)
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.rowCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "rowCallback": function( row, data, displayIndex, displayIndexFull ) {
		 *          // Bold the grade for all 'A' grade browsers
		 *          if ( data[4] == "A" ) {
		 *            $('td:eq(4)', row).html( '<b>A</b>' );
		 *          }
		 *        }
		 *      } );
		 *    } );
		 */
		"fnRowCallback": null,
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * This parameter allows you to override the default function which obtains
		 * the data from the server so something more suitable for your application.
		 * For example you could use POST data, or pull information from a Gears or
		 * AIR database.
		 *  @type function
		 *  @member
		 *  @param {string} source HTTP source to obtain the data from (`ajax`)
		 *  @param {array} data A key/value pair object containing the data to send
		 *    to the server
		 *  @param {function} callback to be called on completion of the data get
		 *    process that will draw the data on the page.
		 *  @param {object} settings DataTables settings object
		 *
		 *  @dtopt Callbacks
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverData
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"fnServerData": null,
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 *  It is often useful to send extra data to the server when making an Ajax
		 * request - for example custom filtering information, and this callback
		 * function makes it trivial to send extra information to the server. The
		 * passed in parameter is the data set that has been constructed by
		 * DataTables, and you can add to this or modify it as you require.
		 *  @type function
		 *  @param {array} data Data array (array of objects which are name/value
		 *    pairs) that has been constructed by DataTables and will be sent to the
		 *    server. In the case of Ajax sourced data with server-side processing
		 *    this will be an empty array, for server-side processing there will be a
		 *    significant number of parameters!
		 *  @returns {undefined} Ensure that you modify the data array passed in,
		 *    as this is passed by reference.
		 *
		 *  @dtopt Callbacks
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverParams
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"fnServerParams": null,
	
	
		/**
		 * Load the table state. With this function you can define from where, and how, the
		 * state of a table is loaded. By default DataTables will load from `localStorage`
		 * but you might wish to use a server-side database or cookies.
		 *  @type function
		 *  @member
		 *  @param {object} settings DataTables settings object
		 *  @param {object} callback Callback that can be executed when done. It
		 *    should be passed the loaded state object.
		 *  @return {object} The DataTables state object to be loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoadCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadCallback": function (settings, callback) {
		 *          $.ajax( {
		 *            "url": "/state_load",
		 *            "dataType": "json",
		 *            "success": function (json) {
		 *              callback( json );
		 *            }
		 *          } );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoadCallback": function ( settings ) {
			try {
				return JSON.parse(
					(settings.iStateDuration === -1 ? sessionStorage : localStorage).getItem(
						'DataTables_'+settings.sInstance+'_'+location.pathname
					)
				);
			} catch (e) {}
		},
	
	
		/**
		 * Callback which allows modification of the saved state prior to loading that state.
		 * This callback is called when the table is loading state from the stored data, but
		 * prior to the settings object being modified by the saved state. Note that for
		 * plug-in authors, you should use the `stateLoadParams` event to load parameters for
		 * a plug-in.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object that is to be loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoadParams
		 *
		 *  @example
		 *    // Remove a saved filter, so filtering is never loaded
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadParams": function (settings, data) {
		 *          data.oSearch.sSearch = "";
		 *        }
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Disallow state loading by returning false
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoadParams": function (settings, data) {
		 *          return false;
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoadParams": null,
	
	
		/**
		 * Callback that is called when the state has been loaded from the state saving method
		 * and the DataTables settings object has been modified as a result of the loaded state.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object that was loaded
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateLoaded
		 *
		 *  @example
		 *    // Show an alert with the filtering value that was saved
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateLoaded": function (settings, data) {
		 *          alert( 'Saved filter was: '+data.oSearch.sSearch );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateLoaded": null,
	
	
		/**
		 * Save the table state. This function allows you to define where and how the state
		 * information for the table is stored By default DataTables will use `localStorage`
		 * but you might wish to use a server-side database or cookies.
		 *  @type function
		 *  @member
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object to be saved
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateSaveCallback
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateSaveCallback": function (settings, data) {
		 *          // Send an Ajax request to the server with the state object
		 *          $.ajax( {
		 *            "url": "/state_save",
		 *            "data": data,
		 *            "dataType": "json",
		 *            "method": "POST"
		 *            "success": function () {}
		 *          } );
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateSaveCallback": function ( settings, data ) {
			try {
				(settings.iStateDuration === -1 ? sessionStorage : localStorage).setItem(
					'DataTables_'+settings.sInstance+'_'+location.pathname,
					JSON.stringify( data )
				);
			} catch (e) {}
		},
	
	
		/**
		 * Callback which allows modification of the state to be saved. Called when the table
		 * has changed state a new state save is required. This method allows modification of
		 * the state saving object prior to actually doing the save, including addition or
		 * other state properties or modification. Note that for plug-in authors, you should
		 * use the `stateSaveParams` event to save parameters for a plug-in.
		 *  @type function
		 *  @param {object} settings DataTables settings object
		 *  @param {object} data The state object to be saved
		 *
		 *  @dtopt Callbacks
		 *  @name DataTable.defaults.stateSaveParams
		 *
		 *  @example
		 *    // Remove a saved filter, so filtering is never saved
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateSave": true,
		 *        "stateSaveParams": function (settings, data) {
		 *          data.oSearch.sSearch = "";
		 *        }
		 *      } );
		 *    } );
		 */
		"fnStateSaveParams": null,
	
	
		/**
		 * Duration for which the saved state information is considered valid. After this period
		 * has elapsed the state will be returned to the default.
		 * Value is given in seconds.
		 *  @type int
		 *  @default 7200 <i>(2 hours)</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.stateDuration
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "stateDuration": 60*60*24; // 1 day
		 *      } );
		 *    } )
		 */
		"iStateDuration": 7200,
	
	
		/**
		 * When enabled DataTables will not make a request to the server for the first
		 * page draw - rather it will use the data already on the page (no sorting etc
		 * will be applied to it), thus saving on an XHR at load time. `deferLoading`
		 * is used to indicate that deferred loading is required, but it is also used
		 * to tell DataTables how many records there are in the full table (allowing
		 * the information element and pagination to be displayed correctly). In the case
		 * where a filtering is applied to the table on initial load, this can be
		 * indicated by giving the parameter as an array, where the first element is
		 * the number of records available after filtering and the second element is the
		 * number of records without filtering (allowing the table information element
		 * to be shown correctly).
		 *  @type int | array
		 *  @default null
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.deferLoading
		 *
		 *  @example
		 *    // 57 records available in the table, no filtering applied
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "scripts/server_processing.php",
		 *        "deferLoading": 57
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // 57 records after filtering, 100 without filtering (an initial filter applied)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "serverSide": true,
		 *        "ajax": "scripts/server_processing.php",
		 *        "deferLoading": [ 57, 100 ],
		 *        "search": {
		 *          "search": "my_filter"
		 *        }
		 *      } );
		 *    } );
		 */
		"iDeferLoading": null,
	
	
		/**
		 * Number of rows to display on a single page when using pagination. If
		 * feature enabled (`lengthChange`) then the end user will be able to override
		 * this to a custom setting using a pop-up menu.
		 *  @type int
		 *  @default 10
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.pageLength
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "pageLength": 50
		 *      } );
		 *    } )
		 */
		"iDisplayLength": 10,
	
	
		/**
		 * Define the starting point for data display when using DataTables with
		 * pagination. Note that this parameter is the number of records, rather than
		 * the page number, so if you have 10 records per page and want to start on
		 * the third page, it should be "20".
		 *  @type int
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.displayStart
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "displayStart": 20
		 *      } );
		 *    } )
		 */
		"iDisplayStart": 0,
	
	
		/**
		 * By default DataTables allows keyboard navigation of the table (sorting, paging,
		 * and filtering) by adding a `tabindex` attribute to the required elements. This
		 * allows you to tab through the controls and press the enter key to activate them.
		 * The tabindex is default 0, meaning that the tab follows the flow of the document.
		 * You can overrule this using this parameter if you wish. Use a value of -1 to
		 * disable built-in keyboard navigation.
		 *  @type int
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.tabIndex
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "tabIndex": 1
		 *      } );
		 *    } );
		 */
		"iTabIndex": 0,
	
	
		/**
		 * Classes that DataTables assigns to the various components and features
		 * that it adds to the HTML table. This allows classes to be configured
		 * during initialisation in addition to through the static
		 * {@link DataTable.ext.oStdClasses} object).
		 *  @namespace
		 *  @name DataTable.defaults.classes
		 */
		"oClasses": {},
	
	
		/**
		 * All strings that DataTables uses in the user interface that it creates
		 * are defined in this object, allowing you to modified them individually or
		 * completely replace them all as required.
		 *  @namespace
		 *  @name DataTable.defaults.language
		 */
		"oLanguage": {
			/**
			 * Strings that are used for WAI-ARIA labels and controls only (these are not
			 * actually visible on the page, but will be read by screenreaders, and thus
			 * must be internationalised as well).
			 *  @namespace
			 *  @name DataTable.defaults.language.aria
			 */
			"oAria": {
				/**
				 * ARIA label that is added to the table headers when the column may be
				 * sorted ascending by activing the column (click or return when focused).
				 * Note that the column header is prefixed to this string.
				 *  @type string
				 *  @default : activate to sort column ascending
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.aria.sortAscending
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "aria": {
				 *            "sortAscending": " - click/return to sort ascending"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sSortAscending": ": activate to sort column ascending",
	
				/**
				 * ARIA label that is added to the table headers when the column may be
				 * sorted descending by activing the column (click or return when focused).
				 * Note that the column header is prefixed to this string.
				 *  @type string
				 *  @default : activate to sort column ascending
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.aria.sortDescending
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "aria": {
				 *            "sortDescending": " - click/return to sort descending"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sSortDescending": ": activate to sort column descending"
			},
	
			/**
			 * Pagination string used by DataTables for the built-in pagination
			 * control types.
			 *  @namespace
			 *  @name DataTable.defaults.language.paginate
			 */
			"oPaginate": {
				/**
				 * Text to use when using the 'full_numbers' type of pagination for the
				 * button to take the user to the first page.
				 *  @type string
				 *  @default First
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.first
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "first": "First page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sFirst": "First",
	
	
				/**
				 * Text to use when using the 'full_numbers' type of pagination for the
				 * button to take the user to the last page.
				 *  @type string
				 *  @default Last
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.last
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "last": "Last page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sLast": "Last",
	
	
				/**
				 * Text to use for the 'next' pagination button (to take the user to the
				 * next page).
				 *  @type string
				 *  @default Next
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.next
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "next": "Next page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sNext": "Next",
	
	
				/**
				 * Text to use for the 'previous' pagination button (to take the user to
				 * the previous page).
				 *  @type string
				 *  @default Previous
				 *
				 *  @dtopt Language
				 *  @name DataTable.defaults.language.paginate.previous
				 *
				 *  @example
				 *    $(document).ready( function() {
				 *      $('#example').dataTable( {
				 *        "language": {
				 *          "paginate": {
				 *            "previous": "Previous page"
				 *          }
				 *        }
				 *      } );
				 *    } );
				 */
				"sPrevious": "Previous"
			},
	
			/**
			 * This string is shown in preference to `zeroRecords` when the table is
			 * empty of data (regardless of filtering). Note that this is an optional
			 * parameter - if it is not given, the value of `zeroRecords` will be used
			 * instead (either the default or given value).
			 *  @type string
			 *  @default No data available in table
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.emptyTable
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "emptyTable": "No data available in table"
			 *        }
			 *      } );
			 *    } );
			 */
			"sEmptyTable": "No data available in table",
	
	
			/**
			 * This string gives information to the end user about the information
			 * that is current on display on the page. The following tokens can be
			 * used in the string and will be dynamically replaced as the table
			 * display updates. This tokens can be placed anywhere in the string, or
			 * removed as needed by the language requires:
			 *
			 * * `\_START\_` - Display index of the first record on the current page
			 * * `\_END\_` - Display index of the last record on the current page
			 * * `\_TOTAL\_` - Number of records in the table after filtering
			 * * `\_MAX\_` - Number of records in the table without filtering
			 * * `\_PAGE\_` - Current page number
			 * * `\_PAGES\_` - Total number of pages of data in the table
			 *
			 *  @type string
			 *  @default Showing _START_ to _END_ of _TOTAL_ entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.info
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "info": "Showing page _PAGE_ of _PAGES_"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
	
	
			/**
			 * Display information string for when the table is empty. Typically the
			 * format of this string should match `info`.
			 *  @type string
			 *  @default Showing 0 to 0 of 0 entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoEmpty
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoEmpty": "No entries to show"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoEmpty": "Showing 0 to 0 of 0 entries",
	
	
			/**
			 * When a user filters the information in a table, this string is appended
			 * to the information (`info`) to give an idea of how strong the filtering
			 * is. The variable _MAX_ is dynamically updated.
			 *  @type string
			 *  @default (filtered from _MAX_ total entries)
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoFiltered
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoFiltered": " - filtering from _MAX_ records"
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoFiltered": "(filtered from _MAX_ total entries)",
	
	
			/**
			 * If can be useful to append extra information to the info string at times,
			 * and this variable does exactly that. This information will be appended to
			 * the `info` (`infoEmpty` and `infoFiltered` in whatever combination they are
			 * being used) at all times.
			 *  @type string
			 *  @default <i>Empty string</i>
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.infoPostFix
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "infoPostFix": "All records shown are derived from real information."
			 *        }
			 *      } );
			 *    } );
			 */
			"sInfoPostFix": "",
	
	
			/**
			 * This decimal place operator is a little different from the other
			 * language options since DataTables doesn't output floating point
			 * numbers, so it won't ever use this for display of a number. Rather,
			 * what this parameter does is modify the sort methods of the table so
			 * that numbers which are in a format which has a character other than
			 * a period (`.`) as a decimal place will be sorted numerically.
			 *
			 * Note that numbers with different decimal places cannot be shown in
			 * the same table and still be sortable, the table must be consistent.
			 * However, multiple different tables on the page can use different
			 * decimal place characters.
			 *  @type string
			 *  @default 
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.decimal
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "decimal": ","
			 *          "thousands": "."
			 *        }
			 *      } );
			 *    } );
			 */
			"sDecimal": "",
	
	
			/**
			 * DataTables has a build in number formatter (`formatNumber`) which is
			 * used to format large numbers that are used in the table information.
			 * By default a comma is used, but this can be trivially changed to any
			 * character you wish with this parameter.
			 *  @type string
			 *  @default ,
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.thousands
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "thousands": "'"
			 *        }
			 *      } );
			 *    } );
			 */
			"sThousands": ",",
	
	
			/**
			 * Detail the action that will be taken when the drop down menu for the
			 * pagination length option is changed. The '_MENU_' variable is replaced
			 * with a default select list of 10, 25, 50 and 100, and can be replaced
			 * with a custom select box if required.
			 *  @type string
			 *  @default Show _MENU_ entries
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.lengthMenu
			 *
			 *  @example
			 *    // Language change only
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "lengthMenu": "Display _MENU_ records"
			 *        }
			 *      } );
			 *    } );
			 *
			 *  @example
			 *    // Language and options change
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "lengthMenu": 'Display <select>'+
			 *            '<option value="10">10</option>'+
			 *            '<option value="20">20</option>'+
			 *            '<option value="30">30</option>'+
			 *            '<option value="40">40</option>'+
			 *            '<option value="50">50</option>'+
			 *            '<option value="-1">All</option>'+
			 *            '</select> records'
			 *        }
			 *      } );
			 *    } );
			 */
			"sLengthMenu": "Show _MENU_ entries",
	
	
			/**
			 * When using Ajax sourced data and during the first draw when DataTables is
			 * gathering the data, this message is shown in an empty row in the table to
			 * indicate to the end user the the data is being loaded. Note that this
			 * parameter is not used when loading data by server-side processing, just
			 * Ajax sourced data with client-side processing.
			 *  @type string
			 *  @default Loading...
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.loadingRecords
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "loadingRecords": "Please wait - loading..."
			 *        }
			 *      } );
			 *    } );
			 */
			"sLoadingRecords": "Loading...",
	
	
			/**
			 * Text which is displayed when the table is processing a user action
			 * (usually a sort command or similar).
			 *  @type string
			 *  @default Processing...
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.processing
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "processing": "DataTables is currently busy"
			 *        }
			 *      } );
			 *    } );
			 */
			"sProcessing": "Processing...",
	
	
			/**
			 * Details the actions that will be taken when the user types into the
			 * filtering input text box. The variable "_INPUT_", if used in the string,
			 * is replaced with the HTML text box for the filtering input allowing
			 * control over where it appears in the string. If "_INPUT_" is not given
			 * then the input box is appended to the string automatically.
			 *  @type string
			 *  @default Search:
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.search
			 *
			 *  @example
			 *    // Input text box will be appended at the end automatically
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "search": "Filter records:"
			 *        }
			 *      } );
			 *    } );
			 *
			 *  @example
			 *    // Specify where the filter should appear
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "search": "Apply filter _INPUT_ to table"
			 *        }
			 *      } );
			 *    } );
			 */
			"sSearch": "Search:",
	
	
			/**
			 * Assign a `placeholder` attribute to the search `input` element
			 *  @type string
			 *  @default 
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.searchPlaceholder
			 */
			"sSearchPlaceholder": "",
	
	
			/**
			 * All of the language information can be stored in a file on the
			 * server-side, which DataTables will look up if this parameter is passed.
			 * It must store the URL of the language file, which is in a JSON format,
			 * and the object has the same properties as the oLanguage object in the
			 * initialiser object (i.e. the above parameters). Please refer to one of
			 * the example language files to see how this works in action.
			 *  @type string
			 *  @default <i>Empty string - i.e. disabled</i>
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.url
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "url": "http://www.sprymedia.co.uk/dataTables/lang.txt"
			 *        }
			 *      } );
			 *    } );
			 */
			"sUrl": "",
	
	
			/**
			 * Text shown inside the table records when the is no information to be
			 * displayed after filtering. `emptyTable` is shown when there is simply no
			 * information in the table at all (regardless of filtering).
			 *  @type string
			 *  @default No matching records found
			 *
			 *  @dtopt Language
			 *  @name DataTable.defaults.language.zeroRecords
			 *
			 *  @example
			 *    $(document).ready( function() {
			 *      $('#example').dataTable( {
			 *        "language": {
			 *          "zeroRecords": "No records to display"
			 *        }
			 *      } );
			 *    } );
			 */
			"sZeroRecords": "No matching records found"
		},
	
	
		/**
		 * This parameter allows you to have define the global filtering state at
		 * initialisation time. As an object the `search` parameter must be
		 * defined, but all other parameters are optional. When `regex` is true,
		 * the search string will be treated as a regular expression, when false
		 * (default) it will be treated as a straight string. When `smart`
		 * DataTables will use it's smart filtering methods (to word match at
		 * any point in the data), when false this will not be done.
		 *  @namespace
		 *  @extends DataTable.models.oSearch
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.search
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "search": {"search": "Initial search"}
		 *      } );
		 *    } )
		 */
		"oSearch": $.extend( {}, DataTable.models.oSearch ),
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * By default DataTables will look for the property `data` (or `aaData` for
		 * compatibility with DataTables 1.9-) when obtaining data from an Ajax
		 * source or for server-side processing - this parameter allows that
		 * property to be changed. You can use Javascript dotted object notation to
		 * get a data source for multiple levels of nesting.
		 *  @type string
		 *  @default data
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.ajaxDataProp
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sAjaxDataProp": "data",
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * You can instruct DataTables to load data from an external
		 * source using this parameter (use aData if you want to pass data in you
		 * already have). Simply provide a url a JSON object can be obtained from.
		 *  @type string
		 *  @default null
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.ajaxSource
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sAjaxSource": null,
	
	
		/**
		 * This initialisation variable allows you to specify exactly where in the
		 * DOM you want DataTables to inject the various controls it adds to the page
		 * (for example you might want the pagination controls at the top of the
		 * table). DIV elements (with or without a custom class) can also be added to
		 * aid styling. The follow syntax is used:
		 *   <ul>
		 *     <li>The following options are allowed:
		 *       <ul>
		 *         <li>'l' - Length changing</li>
		 *         <li>'f' - Filtering input</li>
		 *         <li>'t' - The table!</li>
		 *         <li>'i' - Information</li>
		 *         <li>'p' - Pagination</li>
		 *         <li>'r' - pRocessing</li>
		 *       </ul>
		 *     </li>
		 *     <li>The following constants are allowed:
		 *       <ul>
		 *         <li>'H' - jQueryUI theme "header" classes ('fg-toolbar ui-widget-header ui-corner-tl ui-corner-tr ui-helper-clearfix')</li>
		 *         <li>'F' - jQueryUI theme "footer" classes ('fg-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix')</li>
		 *       </ul>
		 *     </li>
		 *     <li>The following syntax is expected:
		 *       <ul>
		 *         <li>'&lt;' and '&gt;' - div elements</li>
		 *         <li>'&lt;"class" and '&gt;' - div with a class</li>
		 *         <li>'&lt;"#id" and '&gt;' - div with an ID</li>
		 *       </ul>
		 *     </li>
		 *     <li>Examples:
		 *       <ul>
		 *         <li>'&lt;"wrapper"flipt&gt;'</li>
		 *         <li>'&lt;lf&lt;t&gt;ip&gt;'</li>
		 *       </ul>
		 *     </li>
		 *   </ul>
		 *  @type string
		 *  @default lfrtip <i>(when `jQueryUI` is false)</i> <b>or</b>
		 *    <"H"lfr>t<"F"ip> <i>(when `jQueryUI` is true)</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.dom
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "dom": '&lt;"top"i&gt;rt&lt;"bottom"flp&gt;&lt;"clear"&gt;'
		 *      } );
		 *    } );
		 */
		"sDom": "lfrtip",
	
	
		/**
		 * Search delay option. This will throttle full table searches that use the
		 * DataTables provided search input element (it does not effect calls to
		 * `dt-api search()`, providing a delay before the search is made.
		 *  @type integer
		 *  @default 0
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.searchDelay
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "searchDelay": 200
		 *      } );
		 *    } )
		 */
		"searchDelay": null,
	
	
		/**
		 * DataTables features six different built-in options for the buttons to
		 * display for pagination control:
		 *
		 * * `numbers` - Page number buttons only
		 * * `simple` - 'Previous' and 'Next' buttons only
		 * * 'simple_numbers` - 'Previous' and 'Next' buttons, plus page numbers
		 * * `full` - 'First', 'Previous', 'Next' and 'Last' buttons
		 * * `full_numbers` - 'First', 'Previous', 'Next' and 'Last' buttons, plus page numbers
		 * * `first_last_numbers` - 'First' and 'Last' buttons, plus page numbers
		 *  
		 * Further methods can be added using {@link DataTable.ext.oPagination}.
		 *  @type string
		 *  @default simple_numbers
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.pagingType
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "pagingType": "full_numbers"
		 *      } );
		 *    } )
		 */
		"sPaginationType": "simple_numbers",
	
	
		/**
		 * Enable horizontal scrolling. When a table is too wide to fit into a
		 * certain layout, or you have a large number of columns in the table, you
		 * can enable x-scrolling to show the table in a viewport, which can be
		 * scrolled. This property can be `true` which will allow the table to
		 * scroll horizontally when needed, or any CSS unit, or a number (in which
		 * case it will be treated as a pixel measurement). Setting as simply `true`
		 * is recommended.
		 *  @type boolean|string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.scrollX
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollX": true,
		 *        "scrollCollapse": true
		 *      } );
		 *    } );
		 */
		"sScrollX": "",
	
	
		/**
		 * This property can be used to force a DataTable to use more width than it
		 * might otherwise do when x-scrolling is enabled. For example if you have a
		 * table which requires to be well spaced, this parameter is useful for
		 * "over-sizing" the table, and thus forcing scrolling. This property can by
		 * any CSS unit, or a number (in which case it will be treated as a pixel
		 * measurement).
		 *  @type string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Options
		 *  @name DataTable.defaults.scrollXInner
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollX": "100%",
		 *        "scrollXInner": "110%"
		 *      } );
		 *    } );
		 */
		"sScrollXInner": "",
	
	
		/**
		 * Enable vertical scrolling. Vertical scrolling will constrain the DataTable
		 * to the given height, and enable scrolling for any data which overflows the
		 * current viewport. This can be used as an alternative to paging to display
		 * a lot of data in a small area (although paging and scrolling can both be
		 * enabled at the same time). This property can be any CSS unit, or a number
		 * (in which case it will be treated as a pixel measurement).
		 *  @type string
		 *  @default <i>blank string - i.e. disabled</i>
		 *
		 *  @dtopt Features
		 *  @name DataTable.defaults.scrollY
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "scrollY": "200px",
		 *        "paginate": false
		 *      } );
		 *    } );
		 */
		"sScrollY": "",
	
	
		/**
		 * __Deprecated__ The functionality provided by this parameter has now been
		 * superseded by that provided through `ajax`, which should be used instead.
		 *
		 * Set the HTTP method that is used to make the Ajax call for server-side
		 * processing or Ajax sourced data.
		 *  @type string
		 *  @default GET
		 *
		 *  @dtopt Options
		 *  @dtopt Server-side
		 *  @name DataTable.defaults.serverMethod
		 *
		 *  @deprecated 1.10. Please use `ajax` for this functionality now.
		 */
		"sServerMethod": "GET",
	
	
		/**
		 * DataTables makes use of renderers when displaying HTML elements for
		 * a table. These renderers can be added or modified by plug-ins to
		 * generate suitable mark-up for a site. For example the Bootstrap
		 * integration plug-in for DataTables uses a paging button renderer to
		 * display pagination buttons in the mark-up required by Bootstrap.
		 *
		 * For further information about the renderers available see
		 * DataTable.ext.renderer
		 *  @type string|object
		 *  @default null
		 *
		 *  @name DataTable.defaults.renderer
		 *
		 */
		"renderer": null,
	
	
		/**
		 * Set the data property name that DataTables should use to get a row's id
		 * to set as the `id` property in the node.
		 *  @type string
		 *  @default DT_RowId
		 *
		 *  @name DataTable.defaults.rowId
		 */
		"rowId": "DT_RowId"
	};
	
	_fnHungarianMap( DataTable.defaults );
	
	
	
	/*
	 * Developer note - See note in model.defaults.js about the use of Hungarian
	 * notation and camel case.
	 */
	
	/**
	 * Column options that can be given to DataTables at initialisation time.
	 *  @namespace
	 */
	DataTable.defaults.column = {
		/**
		 * Define which column(s) an order will occur on for this column. This
		 * allows a column's ordering to take multiple columns into account when
		 * doing a sort or use the data from a different column. For example first
		 * name / last name columns make sense to do a multi-column sort over the
		 * two columns.
		 *  @type array|int
		 *  @default null <i>Takes the value of the column index automatically</i>
		 *
		 *  @name DataTable.defaults.column.orderData
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderData": [ 0, 1 ], "targets": [ 0 ] },
		 *          { "orderData": [ 1, 0 ], "targets": [ 1 ] },
		 *          { "orderData": 2, "targets": [ 2 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "orderData": [ 0, 1 ] },
		 *          { "orderData": [ 1, 0 ] },
		 *          { "orderData": 2 },
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"aDataSort": null,
		"iDataSort": -1,
	
	
		/**
		 * You can control the default ordering direction, and even alter the
		 * behaviour of the sort handler (i.e. only allow ascending ordering etc)
		 * using this parameter.
		 *  @type array
		 *  @default [ 'asc', 'desc' ]
		 *
		 *  @name DataTable.defaults.column.orderSequence
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderSequence": [ "asc" ], "targets": [ 1 ] },
		 *          { "orderSequence": [ "desc", "asc", "asc" ], "targets": [ 2 ] },
		 *          { "orderSequence": [ "desc" ], "targets": [ 3 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          { "orderSequence": [ "asc" ] },
		 *          { "orderSequence": [ "desc", "asc", "asc" ] },
		 *          { "orderSequence": [ "desc" ] },
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"asSorting": [ 'asc', 'desc' ],
	
	
		/**
		 * Enable or disable filtering on the data in this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.searchable
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "searchable": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "searchable": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bSearchable": true,
	
	
		/**
		 * Enable or disable ordering on this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.orderable
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderable": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "orderable": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bSortable": true,
	
	
		/**
		 * Enable or disable the display of this column.
		 *  @type boolean
		 *  @default true
		 *
		 *  @name DataTable.defaults.column.visible
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "visible": false, "targets": [ 0 ] }
		 *        ] } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "visible": false },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ] } );
		 *    } );
		 */
		"bVisible": true,
	
	
		/**
		 * Developer definable function that is called whenever a cell is created (Ajax source,
		 * etc) or processed for input (DOM source). This can be used as a compliment to mRender
		 * allowing you to modify the DOM element (add background colour for example) when the
		 * element is available.
		 *  @type function
		 *  @param {element} td The TD node that has been created
		 *  @param {*} cellData The Data for the cell
		 *  @param {array|object} rowData The data for the whole row
		 *  @param {int} row The row index for the aoData data store
		 *  @param {int} col The column index for aoColumns
		 *
		 *  @name DataTable.defaults.column.createdCell
		 *  @dtopt Columns
		 *
		 *  @example
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [3],
		 *          "createdCell": function (td, cellData, rowData, row, col) {
		 *            if ( cellData == "1.7" ) {
		 *              $(td).css('color', 'blue')
		 *            }
		 *          }
		 *        } ]
		 *      });
		 *    } );
		 */
		"fnCreatedCell": null,
	
	
		/**
		 * This parameter has been replaced by `data` in DataTables to ensure naming
		 * consistency. `dataProp` can still be used, as there is backwards
		 * compatibility in DataTables for this option, but it is strongly
		 * recommended that you use `data` in preference to `dataProp`.
		 *  @name DataTable.defaults.column.dataProp
		 */
	
	
		/**
		 * This property can be used to read data from any data source property,
		 * including deeply nested objects / properties. `data` can be given in a
		 * number of different ways which effect its behaviour:
		 *
		 * * `integer` - treated as an array index for the data source. This is the
		 *   default that DataTables uses (incrementally increased for each column).
		 * * `string` - read an object property from the data source. There are
		 *   three 'special' options that can be used in the string to alter how
		 *   DataTables reads the data from the source object:
		 *    * `.` - Dotted Javascript notation. Just as you use a `.` in
		 *      Javascript to read from nested objects, so to can the options
		 *      specified in `data`. For example: `browser.version` or
		 *      `browser.name`. If your object parameter name contains a period, use
		 *      `\\` to escape it - i.e. `first\\.name`.
		 *    * `[]` - Array notation. DataTables can automatically combine data
		 *      from and array source, joining the data with the characters provided
		 *      between the two brackets. For example: `name[, ]` would provide a
		 *      comma-space separated list from the source array. If no characters
		 *      are provided between the brackets, the original array source is
		 *      returned.
		 *    * `()` - Function notation. Adding `()` to the end of a parameter will
		 *      execute a function of the name given. For example: `browser()` for a
		 *      simple function on the data source, `browser.version()` for a
		 *      function in a nested property or even `browser().version` to get an
		 *      object property if the function called returns an object. Note that
		 *      function notation is recommended for use in `render` rather than
		 *      `data` as it is much simpler to use as a renderer.
		 * * `null` - use the original data source for the row rather than plucking
		 *   data directly from it. This action has effects on two other
		 *   initialisation options:
		 *    * `defaultContent` - When null is given as the `data` option and
		 *      `defaultContent` is specified for the column, the value defined by
		 *      `defaultContent` will be used for the cell.
		 *    * `render` - When null is used for the `data` option and the `render`
		 *      option is specified for the column, the whole data source for the
		 *      row is used for the renderer.
		 * * `function` - the function given will be executed whenever DataTables
		 *   needs to set or get the data for a cell in the column. The function
		 *   takes three parameters:
		 *    * Parameters:
		 *      * `{array|object}` The data source for the row
		 *      * `{string}` The type call data requested - this will be 'set' when
		 *        setting data or 'filter', 'display', 'type', 'sort' or undefined
		 *        when gathering data. Note that when `undefined` is given for the
		 *        type DataTables expects to get the raw data for the object back<
		 *      * `{*}` Data to set when the second parameter is 'set'.
		 *    * Return:
		 *      * The return value from the function is not required when 'set' is
		 *        the type of call, but otherwise the return is what will be used
		 *        for the data requested.
		 *
		 * Note that `data` is a getter and setter option. If you just require
		 * formatting of data for output, you will likely want to use `render` which
		 * is simply a getter and thus simpler to use.
		 *
		 * Note that prior to DataTables 1.9.2 `data` was called `mDataProp`. The
		 * name change reflects the flexibility of this property and is consistent
		 * with the naming of mRender. If 'mDataProp' is given, then it will still
		 * be used by DataTables, as it automatically maps the old name to the new
		 * if required.
		 *
		 *  @type string|int|function|null
		 *  @default null <i>Use automatically calculated column index</i>
		 *
		 *  @name DataTable.defaults.column.data
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Read table data from objects
		 *    // JSON structure for each row:
		 *    //   {
		 *    //      "engine": {value},
		 *    //      "browser": {value},
		 *    //      "platform": {value},
		 *    //      "version": {value},
		 *    //      "grade": {value}
		 *    //   }
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/objects.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          { "data": "platform" },
		 *          { "data": "version" },
		 *          { "data": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Read information from deeply nested objects
		 *    // JSON structure for each row:
		 *    //   {
		 *    //      "engine": {value},
		 *    //      "browser": {value},
		 *    //      "platform": {
		 *    //         "inner": {value}
		 *    //      },
		 *    //      "details": [
		 *    //         {value}, {value}
		 *    //      ]
		 *    //   }
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/deep.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          { "data": "platform.inner" },
		 *          { "data": "platform.details.0" },
		 *          { "data": "platform.details.1" }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `data` as a function to provide different information for
		 *    // sorting, filtering and display. In this case, currency (price)
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": function ( source, type, val ) {
		 *            if (type === 'set') {
		 *              source.price = val;
		 *              // Store the computed dislay and filter values for efficiency
		 *              source.price_display = val=="" ? "" : "$"+numberFormat(val);
		 *              source.price_filter  = val=="" ? "" : "$"+numberFormat(val)+" "+val;
		 *              return;
		 *            }
		 *            else if (type === 'display') {
		 *              return source.price_display;
		 *            }
		 *            else if (type === 'filter') {
		 *              return source.price_filter;
		 *            }
		 *            // 'sort', 'type' and undefined all just use the integer
		 *            return source.price;
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using default content
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null,
		 *          "defaultContent": "Click to edit"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using array notation - outputting a list from an array
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": "name[, ]"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 */
		"mData": null,
	
	
		/**
		 * This property is the rendering partner to `data` and it is suggested that
		 * when you want to manipulate data for display (including filtering,
		 * sorting etc) without altering the underlying data for the table, use this
		 * property. `render` can be considered to be the the read only companion to
		 * `data` which is read / write (then as such more complex). Like `data`
		 * this option can be given in a number of different ways to effect its
		 * behaviour:
		 *
		 * * `integer` - treated as an array index for the data source. This is the
		 *   default that DataTables uses (incrementally increased for each column).
		 * * `string` - read an object property from the data source. There are
		 *   three 'special' options that can be used in the string to alter how
		 *   DataTables reads the data from the source object:
		 *    * `.` - Dotted Javascript notation. Just as you use a `.` in
		 *      Javascript to read from nested objects, so to can the options
		 *      specified in `data`. For example: `browser.version` or
		 *      `browser.name`. If your object parameter name contains a period, use
		 *      `\\` to escape it - i.e. `first\\.name`.
		 *    * `[]` - Array notation. DataTables can automatically combine data
		 *      from and array source, joining the data with the characters provided
		 *      between the two brackets. For example: `name[, ]` would provide a
		 *      comma-space separated list from the source array. If no characters
		 *      are provided between the brackets, the original array source is
		 *      returned.
		 *    * `()` - Function notation. Adding `()` to the end of a parameter will
		 *      execute a function of the name given. For example: `browser()` for a
		 *      simple function on the data source, `browser.version()` for a
		 *      function in a nested property or even `browser().version` to get an
		 *      object property if the function called returns an object.
		 * * `object` - use different data for the different data types requested by
		 *   DataTables ('filter', 'display', 'type' or 'sort'). The property names
		 *   of the object is the data type the property refers to and the value can
		 *   defined using an integer, string or function using the same rules as
		 *   `render` normally does. Note that an `_` option _must_ be specified.
		 *   This is the default value to use if you haven't specified a value for
		 *   the data type requested by DataTables.
		 * * `function` - the function given will be executed whenever DataTables
		 *   needs to set or get the data for a cell in the column. The function
		 *   takes three parameters:
		 *    * Parameters:
		 *      * {array|object} The data source for the row (based on `data`)
		 *      * {string} The type call data requested - this will be 'filter',
		 *        'display', 'type' or 'sort'.
		 *      * {array|object} The full data source for the row (not based on
		 *        `data`)
		 *    * Return:
		 *      * The return value from the function is what will be used for the
		 *        data requested.
		 *
		 *  @type string|int|function|object|null
		 *  @default null Use the data source value.
		 *
		 *  @name DataTable.defaults.column.render
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Create a comma separated list from an array of objects
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "ajaxSource": "sources/deep.txt",
		 *        "columns": [
		 *          { "data": "engine" },
		 *          { "data": "browser" },
		 *          {
		 *            "data": "platform",
		 *            "render": "[, ].name"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Execute a function to obtain data
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null, // Use the full data source object for the renderer's source
		 *          "render": "browserName()"
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // As an object, extracting different data for the different types
		 *    // This would be used with a data source such as:
		 *    //   { "phone": 5552368, "phone_filter": "5552368 555-2368", "phone_display": "555-2368" }
		 *    // Here the `phone` integer is used for sorting and type detection, while `phone_filter`
		 *    // (which has both forms) is used for filtering for if a user inputs either format, while
		 *    // the formatted phone number is the one that is shown in the table.
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": null, // Use the full data source object for the renderer's source
		 *          "render": {
		 *            "_": "phone",
		 *            "filter": "phone_filter",
		 *            "display": "phone_display"
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Use as a function to create a link from the data source
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "data": "download_link",
		 *          "render": function ( data, type, full ) {
		 *            return '<a href="'+data+'">Download</a>';
		 *          }
		 *        } ]
		 *      } );
		 *    } );
		 */
		"mRender": null,
	
	
		/**
		 * Change the cell type created for the column - either TD cells or TH cells. This
		 * can be useful as TH cells have semantic meaning in the table body, allowing them
		 * to act as a header for a row (you may wish to add scope='row' to the TH elements).
		 *  @type string
		 *  @default td
		 *
		 *  @name DataTable.defaults.column.cellType
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Make the first column use TH cells
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [ {
		 *          "targets": [ 0 ],
		 *          "cellType": "th"
		 *        } ]
		 *      } );
		 *    } );
		 */
		"sCellType": "td",
	
	
		/**
		 * Class to give to each cell in this column.
		 *  @type string
		 *  @default <i>Empty string</i>
		 *
		 *  @name DataTable.defaults.column.class
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "class": "my_class", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "class": "my_class" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sClass": "",
	
		/**
		 * When DataTables calculates the column widths to assign to each column,
		 * it finds the longest string in each column and then constructs a
		 * temporary table and reads the widths from that. The problem with this
		 * is that "mmm" is much wider then "iiii", but the latter is a longer
		 * string - thus the calculation can go wrong (doing it properly and putting
		 * it into an DOM object and measuring that is horribly(!) slow). Thus as
		 * a "work around" we provide this option. It will append its value to the
		 * text that is found to be the longest string for the column - i.e. padding.
		 * Generally you shouldn't need this!
		 *  @type string
		 *  @default <i>Empty string<i>
		 *
		 *  @name DataTable.defaults.column.contentPadding
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          null,
		 *          {
		 *            "contentPadding": "mmm"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sContentPadding": "",
	
	
		/**
		 * Allows a default value to be given for a column's data, and will be used
		 * whenever a null data source is encountered (this can be because `data`
		 * is set to null, or because the data source itself is null).
		 *  @type string
		 *  @default null
		 *
		 *  @name DataTable.defaults.column.defaultContent
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          {
		 *            "data": null,
		 *            "defaultContent": "Edit",
		 *            "targets": [ -1 ]
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          null,
		 *          {
		 *            "data": null,
		 *            "defaultContent": "Edit"
		 *          }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sDefaultContent": null,
	
	
		/**
		 * This parameter is only used in DataTables' server-side processing. It can
		 * be exceptionally useful to know what columns are being displayed on the
		 * client side, and to map these to database fields. When defined, the names
		 * also allow DataTables to reorder information from the server if it comes
		 * back in an unexpected order (i.e. if you switch your columns around on the
		 * client-side, your server-side code does not also need updating).
		 *  @type string
		 *  @default <i>Empty string</i>
		 *
		 *  @name DataTable.defaults.column.name
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "name": "engine", "targets": [ 0 ] },
		 *          { "name": "browser", "targets": [ 1 ] },
		 *          { "name": "platform", "targets": [ 2 ] },
		 *          { "name": "version", "targets": [ 3 ] },
		 *          { "name": "grade", "targets": [ 4 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "name": "engine" },
		 *          { "name": "browser" },
		 *          { "name": "platform" },
		 *          { "name": "version" },
		 *          { "name": "grade" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sName": "",
	
	
		/**
		 * Defines a data source type for the ordering which can be used to read
		 * real-time information from the table (updating the internally cached
		 * version) prior to ordering. This allows ordering to occur on user
		 * editable elements such as form inputs.
		 *  @type string
		 *  @default std
		 *
		 *  @name DataTable.defaults.column.orderDataType
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "orderDataType": "dom-text", "targets": [ 2, 3 ] },
		 *          { "type": "numeric", "targets": [ 3 ] },
		 *          { "orderDataType": "dom-select", "targets": [ 4 ] },
		 *          { "orderDataType": "dom-checkbox", "targets": [ 5 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          null,
		 *          null,
		 *          { "orderDataType": "dom-text" },
		 *          { "orderDataType": "dom-text", "type": "numeric" },
		 *          { "orderDataType": "dom-select" },
		 *          { "orderDataType": "dom-checkbox" }
		 *        ]
		 *      } );
		 *    } );
		 */
		"sSortDataType": "std",
	
	
		/**
		 * The title of this column.
		 *  @type string
		 *  @default null <i>Derived from the 'TH' value for this column in the
		 *    original HTML table.</i>
		 *
		 *  @name DataTable.defaults.column.title
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "title": "My column title", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "title": "My column title" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sTitle": null,
	
	
		/**
		 * The type allows you to specify how the data for this column will be
		 * ordered. Four types (string, numeric, date and html (which will strip
		 * HTML tags before ordering)) are currently available. Note that only date
		 * formats understood by Javascript's Date() object will be accepted as type
		 * date. For example: "Mar 26, 2008 5:03 PM". May take the values: 'string',
		 * 'numeric', 'date' or 'html' (by default). Further types can be adding
		 * through plug-ins.
		 *  @type string
		 *  @default null <i>Auto-detected from raw data</i>
		 *
		 *  @name DataTable.defaults.column.type
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "type": "html", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "type": "html" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sType": null,
	
	
		/**
		 * Defining the width of the column, this parameter may take any CSS value
		 * (3em, 20px etc). DataTables applies 'smart' widths to columns which have not
		 * been given a specific width through this interface ensuring that the table
		 * remains readable.
		 *  @type string
		 *  @default null <i>Automatic</i>
		 *
		 *  @name DataTable.defaults.column.width
		 *  @dtopt Columns
		 *
		 *  @example
		 *    // Using `columnDefs`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columnDefs": [
		 *          { "width": "20%", "targets": [ 0 ] }
		 *        ]
		 *      } );
		 *    } );
		 *
		 *  @example
		 *    // Using `columns`
		 *    $(document).ready( function() {
		 *      $('#example').dataTable( {
		 *        "columns": [
		 *          { "width": "20%" },
		 *          null,
		 *          null,
		 *          null,
		 *          null
		 *        ]
		 *      } );
		 *    } );
		 */
		"sWidth": null
	};
	
	_fnHungarianMap( DataTable.defaults.column );
	
	
	
	/**
	 * DataTables settings object - this holds all the information needed for a
	 * given table, including configuration, data and current application of the
	 * table options. DataTables does not have a single instance for each DataTable
	 * with the settings attached to that instance, but rather instances of the
	 * DataTable "class" are created on-the-fly as needed (typically by a
	 * $().dataTable() call) and the settings object is then applied to that
	 * instance.
	 *
	 * Note that this object is related to {@link DataTable.defaults} but this
	 * one is the internal data store for DataTables's cache of columns. It should
	 * NOT be manipulated outside of DataTables. Any configuration should be done
	 * through the initialisation options.
	 *  @namespace
	 *  @todo Really should attach the settings object to individual instances so we
	 *    don't need to create new instances on each $().dataTable() call (if the
	 *    table already exists). It would also save passing oSettings around and
	 *    into every single function. However, this is a very significant
	 *    architecture change for DataTables and will almost certainly break
	 *    backwards compatibility with older installations. This is something that
	 *    will be done in 2.0.
	 */
	DataTable.models.oSettings = {
		/**
		 * Primary features of DataTables and their enablement state.
		 *  @namespace
		 */
		"oFeatures": {
	
			/**
			 * Flag to say if DataTables should automatically try to calculate the
			 * optimum table and columns widths (true) or not (false).
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bAutoWidth": null,
	
			/**
			 * Delay the creation of TR and TD elements until they are actually
			 * needed by a driven page draw. This can give a significant speed
			 * increase for Ajax source and Javascript source data, but makes no
			 * difference at all fro DOM and server-side processing tables.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bDeferRender": null,
	
			/**
			 * Enable filtering on the table or not. Note that if this is disabled
			 * then there is no filtering at all on the table, including fnFilter.
			 * To just remove the filtering input use sDom and remove the 'f' option.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bFilter": null,
	
			/**
			 * Table information element (the 'Showing x of y records' div) enable
			 * flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bInfo": null,
	
			/**
			 * Present a user control allowing the end user to change the page size
			 * when pagination is enabled.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bLengthChange": null,
	
			/**
			 * Pagination enabled or not. Note that if this is disabled then length
			 * changing must also be disabled.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bPaginate": null,
	
			/**
			 * Processing indicator enable flag whenever DataTables is enacting a
			 * user request - typically an Ajax request for server-side processing.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bProcessing": null,
	
			/**
			 * Server-side processing enabled flag - when enabled DataTables will
			 * get all data from the server for every draw - there is no filtering,
			 * sorting or paging done on the client-side.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bServerSide": null,
	
			/**
			 * Sorting enablement flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSort": null,
	
			/**
			 * Multi-column sorting
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSortMulti": null,
	
			/**
			 * Apply a class to the columns which are being sorted to provide a
			 * visual highlight or not. This can slow things down when enabled since
			 * there is a lot of DOM interaction.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bSortClasses": null,
	
			/**
			 * State saving enablement flag.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bStateSave": null
		},
	
	
		/**
		 * Scrolling settings for a table.
		 *  @namespace
		 */
		"oScroll": {
			/**
			 * When the table is shorter in height than sScrollY, collapse the
			 * table container down to the height of the table (when true).
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type boolean
			 */
			"bCollapse": null,
	
			/**
			 * Width of the scrollbar for the web-browser's platform. Calculated
			 * during table initialisation.
			 *  @type int
			 *  @default 0
			 */
			"iBarWidth": 0,
	
			/**
			 * Viewport width for horizontal scrolling. Horizontal scrolling is
			 * disabled if an empty string.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 */
			"sX": null,
	
			/**
			 * Width to expand the table to when using x-scrolling. Typically you
			 * should not need to use this.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 *  @deprecated
			 */
			"sXInner": null,
	
			/**
			 * Viewport height for vertical scrolling. Vertical scrolling is disabled
			 * if an empty string.
			 * Note that this parameter will be set by the initialisation routine. To
			 * set a default use {@link DataTable.defaults}.
			 *  @type string
			 */
			"sY": null
		},
	
		/**
		 * Language information for the table.
		 *  @namespace
		 *  @extends DataTable.defaults.oLanguage
		 */
		"oLanguage": {
			/**
			 * Information callback function. See
			 * {@link DataTable.defaults.fnInfoCallback}
			 *  @type function
			 *  @default null
			 */
			"fnInfoCallback": null
		},
	
		/**
		 * Browser support parameters
		 *  @namespace
		 */
		"oBrowser": {
			/**
			 * Indicate if the browser incorrectly calculates width:100% inside a
			 * scrolling element (IE6/7)
			 *  @type boolean
			 *  @default false
			 */
			"bScrollOversize": false,
	
			/**
			 * Determine if the vertical scrollbar is on the right or left of the
			 * scrolling container - needed for rtl language layout, although not
			 * all browsers move the scrollbar (Safari).
			 *  @type boolean
			 *  @default false
			 */
			"bScrollbarLeft": false,
	
			/**
			 * Flag for if `getBoundingClientRect` is fully supported or not
			 *  @type boolean
			 *  @default false
			 */
			"bBounding": false,
	
			/**
			 * Browser scrollbar width
			 *  @type integer
			 *  @default 0
			 */
			"barWidth": 0
		},
	
	
		"ajax": null,
	
	
		/**
		 * Array referencing the nodes which are used for the features. The
		 * parameters of this object match what is allowed by sDom - i.e.
		 *   <ul>
		 *     <li>'l' - Length changing</li>
		 *     <li>'f' - Filtering input</li>
		 *     <li>'t' - The table!</li>
		 *     <li>'i' - Information</li>
		 *     <li>'p' - Pagination</li>
		 *     <li>'r' - pRocessing</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aanFeatures": [],
	
		/**
		 * Store data information - see {@link DataTable.models.oRow} for detailed
		 * information.
		 *  @type array
		 *  @default []
		 */
		"aoData": [],
	
		/**
		 * Array of indexes which are in the current display (after filtering etc)
		 *  @type array
		 *  @default []
		 */
		"aiDisplay": [],
	
		/**
		 * Array of indexes for display - no filtering
		 *  @type array
		 *  @default []
		 */
		"aiDisplayMaster": [],
	
		/**
		 * Map of row ids to data indexes
		 *  @type object
		 *  @default {}
		 */
		"aIds": {},
	
		/**
		 * Store information about each column that is in use
		 *  @type array
		 *  @default []
		 */
		"aoColumns": [],
	
		/**
		 * Store information about the table's header
		 *  @type array
		 *  @default []
		 */
		"aoHeader": [],
	
		/**
		 * Store information about the table's footer
		 *  @type array
		 *  @default []
		 */
		"aoFooter": [],
	
		/**
		 * Store the applied global search information in case we want to force a
		 * research or compare the old search to a new one.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @namespace
		 *  @extends DataTable.models.oSearch
		 */
		"oPreviousSearch": {},
	
		/**
		 * Store the applied search for each column - see
		 * {@link DataTable.models.oSearch} for the format that is used for the
		 * filtering information for each column.
		 *  @type array
		 *  @default []
		 */
		"aoPreSearchCols": [],
	
		/**
		 * Sorting that is applied to the table. Note that the inner arrays are
		 * used in the following manner:
		 * <ul>
		 *   <li>Index 0 - column number</li>
		 *   <li>Index 1 - current sorting direction</li>
		 * </ul>
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @todo These inner arrays should really be objects
		 */
		"aaSorting": null,
	
		/**
		 * Sorting that is always applied to the table (i.e. prefixed in front of
		 * aaSorting).
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"aaSortingFixed": [],
	
		/**
		 * Classes to use for the striping of a table.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"asStripeClasses": null,
	
		/**
		 * If restoring a table - we should restore its striping classes as well
		 *  @type array
		 *  @default []
		 */
		"asDestroyStripes": [],
	
		/**
		 * If restoring a table - we should restore its width
		 *  @type int
		 *  @default 0
		 */
		"sDestroyWidth": 0,
	
		/**
		 * Callback functions array for every time a row is inserted (i.e. on a draw).
		 *  @type array
		 *  @default []
		 */
		"aoRowCallback": [],
	
		/**
		 * Callback functions for the header on each draw.
		 *  @type array
		 *  @default []
		 */
		"aoHeaderCallback": [],
	
		/**
		 * Callback function for the footer on each draw.
		 *  @type array
		 *  @default []
		 */
		"aoFooterCallback": [],
	
		/**
		 * Array of callback functions for draw callback functions
		 *  @type array
		 *  @default []
		 */
		"aoDrawCallback": [],
	
		/**
		 * Array of callback functions for row created function
		 *  @type array
		 *  @default []
		 */
		"aoRowCreatedCallback": [],
	
		/**
		 * Callback functions for just before the table is redrawn. A return of
		 * false will be used to cancel the draw.
		 *  @type array
		 *  @default []
		 */
		"aoPreDrawCallback": [],
	
		/**
		 * Callback functions for when the table has been initialised.
		 *  @type array
		 *  @default []
		 */
		"aoInitComplete": [],
	
	
		/**
		 * Callbacks for modifying the settings to be stored for state saving, prior to
		 * saving state.
		 *  @type array
		 *  @default []
		 */
		"aoStateSaveParams": [],
	
		/**
		 * Callbacks for modifying the settings that have been stored for state saving
		 * prior to using the stored values to restore the state.
		 *  @type array
		 *  @default []
		 */
		"aoStateLoadParams": [],
	
		/**
		 * Callbacks for operating on the settings object once the saved state has been
		 * loaded
		 *  @type array
		 *  @default []
		 */
		"aoStateLoaded": [],
	
		/**
		 * Cache the table ID for quick access
		 *  @type string
		 *  @default <i>Empty string</i>
		 */
		"sTableId": "",
	
		/**
		 * The TABLE node for the main table
		 *  @type node
		 *  @default null
		 */
		"nTable": null,
	
		/**
		 * Permanent ref to the thead element
		 *  @type node
		 *  @default null
		 */
		"nTHead": null,
	
		/**
		 * Permanent ref to the tfoot element - if it exists
		 *  @type node
		 *  @default null
		 */
		"nTFoot": null,
	
		/**
		 * Permanent ref to the tbody element
		 *  @type node
		 *  @default null
		 */
		"nTBody": null,
	
		/**
		 * Cache the wrapper node (contains all DataTables controlled elements)
		 *  @type node
		 *  @default null
		 */
		"nTableWrapper": null,
	
		/**
		 * Indicate if when using server-side processing the loading of data
		 * should be deferred until the second draw.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type boolean
		 *  @default false
		 */
		"bDeferLoading": false,
	
		/**
		 * Indicate if all required information has been read in
		 *  @type boolean
		 *  @default false
		 */
		"bInitialised": false,
	
		/**
		 * Information about open rows. Each object in the array has the parameters
		 * 'nTr' and 'nParent'
		 *  @type array
		 *  @default []
		 */
		"aoOpenRows": [],
	
		/**
		 * Dictate the positioning of DataTables' control elements - see
		 * {@link DataTable.model.oInit.sDom}.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default null
		 */
		"sDom": null,
	
		/**
		 * Search delay (in mS)
		 *  @type integer
		 *  @default null
		 */
		"searchDelay": null,
	
		/**
		 * Which type of pagination should be used.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default two_button
		 */
		"sPaginationType": "two_button",
	
		/**
		 * The state duration (for `stateSave`) in seconds.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type int
		 *  @default 0
		 */
		"iStateDuration": 0,
	
		/**
		 * Array of callback functions for state saving. Each array element is an
		 * object with the following parameters:
		 *   <ul>
		 *     <li>function:fn - function to call. Takes two parameters, oSettings
		 *       and the JSON string to save that has been thus far created. Returns
		 *       a JSON string to be inserted into a json object
		 *       (i.e. '"param": [ 0, 1, 2]')</li>
		 *     <li>string:sName - name of callback</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aoStateSave": [],
	
		/**
		 * Array of callback functions for state loading. Each array element is an
		 * object with the following parameters:
		 *   <ul>
		 *     <li>function:fn - function to call. Takes two parameters, oSettings
		 *       and the object stored. May return false to cancel state loading</li>
		 *     <li>string:sName - name of callback</li>
		 *   </ul>
		 *  @type array
		 *  @default []
		 */
		"aoStateLoad": [],
	
		/**
		 * State that was saved. Useful for back reference
		 *  @type object
		 *  @default null
		 */
		"oSavedState": null,
	
		/**
		 * State that was loaded. Useful for back reference
		 *  @type object
		 *  @default null
		 */
		"oLoadedState": null,
	
		/**
		 * Source url for AJAX data for the table.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 *  @default null
		 */
		"sAjaxSource": null,
	
		/**
		 * Property from a given object from which to read the table data from. This
		 * can be an empty string (when not server-side processing), in which case
		 * it is  assumed an an array is given directly.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 */
		"sAjaxDataProp": null,
	
		/**
		 * Note if draw should be blocked while getting data
		 *  @type boolean
		 *  @default true
		 */
		"bAjaxDataGet": true,
	
		/**
		 * The last jQuery XHR object that was used for server-side data gathering.
		 * This can be used for working with the XHR information in one of the
		 * callbacks
		 *  @type object
		 *  @default null
		 */
		"jqXHR": null,
	
		/**
		 * JSON returned from the server in the last Ajax request
		 *  @type object
		 *  @default undefined
		 */
		"json": undefined,
	
		/**
		 * Data submitted as part of the last Ajax request
		 *  @type object
		 *  @default undefined
		 */
		"oAjaxData": undefined,
	
		/**
		 * Function to get the server-side data.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type function
		 */
		"fnServerData": null,
	
		/**
		 * Functions which are called prior to sending an Ajax request so extra
		 * parameters can easily be sent to the server
		 *  @type array
		 *  @default []
		 */
		"aoServerParams": [],
	
		/**
		 * Send the XHR HTTP method - GET or POST (could be PUT or DELETE if
		 * required).
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type string
		 */
		"sServerMethod": null,
	
		/**
		 * Format numbers for display.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type function
		 */
		"fnFormatNumber": null,
	
		/**
		 * List of options that can be used for the user selectable length menu.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type array
		 *  @default []
		 */
		"aLengthMenu": null,
	
		/**
		 * Counter for the draws that the table does. Also used as a tracker for
		 * server-side processing
		 *  @type int
		 *  @default 0
		 */
		"iDraw": 0,
	
		/**
		 * Indicate if a redraw is being done - useful for Ajax
		 *  @type boolean
		 *  @default false
		 */
		"bDrawing": false,
	
		/**
		 * Draw index (iDraw) of the last error when parsing the returned data
		 *  @type int
		 *  @default -1
		 */
		"iDrawError": -1,
	
		/**
		 * Paging display length
		 *  @type int
		 *  @default 10
		 */
		"_iDisplayLength": 10,
	
		/**
		 * Paging start point - aiDisplay index
		 *  @type int
		 *  @default 0
		 */
		"_iDisplayStart": 0,
	
		/**
		 * Server-side processing - number of records in the result set
		 * (i.e. before filtering), Use fnRecordsTotal rather than
		 * this property to get the value of the number of records, regardless of
		 * the server-side processing setting.
		 *  @type int
		 *  @default 0
		 *  @private
		 */
		"_iRecordsTotal": 0,
	
		/**
		 * Server-side processing - number of records in the current display set
		 * (i.e. after filtering). Use fnRecordsDisplay rather than
		 * this property to get the value of the number of records, regardless of
		 * the server-side processing setting.
		 *  @type boolean
		 *  @default 0
		 *  @private
		 */
		"_iRecordsDisplay": 0,
	
		/**
		 * The classes to use for the table
		 *  @type object
		 *  @default {}
		 */
		"oClasses": {},
	
		/**
		 * Flag attached to the settings object so you can check in the draw
		 * callback if filtering has been done in the draw. Deprecated in favour of
		 * events.
		 *  @type boolean
		 *  @default false
		 *  @deprecated
		 */
		"bFiltered": false,
	
		/**
		 * Flag attached to the settings object so you can check in the draw
		 * callback if sorting has been done in the draw. Deprecated in favour of
		 * events.
		 *  @type boolean
		 *  @default false
		 *  @deprecated
		 */
		"bSorted": false,
	
		/**
		 * Indicate that if multiple rows are in the header and there is more than
		 * one unique cell per column, if the top one (true) or bottom one (false)
		 * should be used for sorting / title by DataTables.
		 * Note that this parameter will be set by the initialisation routine. To
		 * set a default use {@link DataTable.defaults}.
		 *  @type boolean
		 */
		"bSortCellsTop": null,
	
		/**
		 * Initialisation object that is used for the table
		 *  @type object
		 *  @default null
		 */
		"oInit": null,
	
		/**
		 * Destroy callback functions - for plug-ins to attach themselves to the
		 * destroy so they can clean up markup and events.
		 *  @type array
		 *  @default []
		 */
		"aoDestroyCallback": [],
	
	
		/**
		 * Get the number of records in the current record set, before filtering
		 *  @type function
		 */
		"fnRecordsTotal": function ()
		{
			return _fnDataSource( this ) == 'ssp' ?
				this._iRecordsTotal * 1 :
				this.aiDisplayMaster.length;
		},
	
		/**
		 * Get the number of records in the current record set, after filtering
		 *  @type function
		 */
		"fnRecordsDisplay": function ()
		{
			return _fnDataSource( this ) == 'ssp' ?
				this._iRecordsDisplay * 1 :
				this.aiDisplay.length;
		},
	
		/**
		 * Get the display end point - aiDisplay index
		 *  @type function
		 */
		"fnDisplayEnd": function ()
		{
			var
				len      = this._iDisplayLength,
				start    = this._iDisplayStart,
				calc     = start + len,
				records  = this.aiDisplay.length,
				features = this.oFeatures,
				paginate = features.bPaginate;
	
			if ( features.bServerSide ) {
				return paginate === false || len === -1 ?
					start + records :
					Math.min( start+len, this._iRecordsDisplay );
			}
			else {
				return ! paginate || calc>records || len===-1 ?
					records :
					calc;
			}
		},
	
		/**
		 * The DataTables object for this table
		 *  @type object
		 *  @default null
		 */
		"oInstance": null,
	
		/**
		 * Unique identifier for each instance of the DataTables object. If there
		 * is an ID on the table node, then it takes that value, otherwise an
		 * incrementing internal counter is used.
		 *  @type string
		 *  @default null
		 */
		"sInstance": null,
	
		/**
		 * tabindex attribute value that is added to DataTables control elements, allowing
		 * keyboard navigation of the table and its controls.
		 */
		"iTabIndex": 0,
	
		/**
		 * DIV container for the footer scrolling table if scrolling
		 */
		"nScrollHead": null,
	
		/**
		 * DIV container for the footer scrolling table if scrolling
		 */
		"nScrollFoot": null,
	
		/**
		 * Last applied sort
		 *  @type array
		 *  @default []
		 */
		"aLastSort": [],
	
		/**
		 * Stored plug-in instances
		 *  @type object
		 *  @default {}
		 */
		"oPlugins": {},
	
		/**
		 * Function used to get a row's id from the row's data
		 *  @type function
		 *  @default null
		 */
		"rowIdFn": null,
	
		/**
		 * Data location where to store a row's id
		 *  @type string
		 *  @default null
		 */
		"rowId": null
	};

	/**
	 * Extension object for DataTables that is used to provide all extension
	 * options.
	 *
	 * Note that the `DataTable.ext` object is available through
	 * `jQuery.fn.dataTable.ext` where it may be accessed and manipulated. It is
	 * also aliased to `jQuery.fn.dataTableExt` for historic reasons.
	 *  @namespace
	 *  @extends DataTable.models.ext
	 */
	
	
	/**
	 * DataTables extensions
	 * 
	 * This namespace acts as a collection area for plug-ins that can be used to
	 * extend DataTables capabilities. Indeed many of the build in methods
	 * use this method to provide their own capabilities (sorting methods for
	 * example).
	 *
	 * Note that this namespace is aliased to `jQuery.fn.dataTableExt` for legacy
	 * reasons
	 *
	 *  @namespace
	 */
	DataTable.ext = _ext = {
		/**
		 * Buttons. For use with the Buttons extension for DataTables. This is
		 * defined here so other extensions can define buttons regardless of load
		 * order. It is _not_ used by DataTables core.
		 *
		 *  @type object
		 *  @default {}
		 */
		buttons: {},
	
	
		/**
		 * Element class names
		 *
		 *  @type object
		 *  @default {}
		 */
		classes: {},
	
	
		/**
		 * DataTables build type (expanded by the download builder)
		 *
		 *  @type string
		 */
		build:"dt/dt-1.10.16/e-1.7.0/b-1.5.1/sl-1.2.4",
	
	
		/**
		 * Error reporting.
		 * 
		 * How should DataTables report an error. Can take the value 'alert',
		 * 'throw', 'none' or a function.
		 *
		 *  @type string|function
		 *  @default alert
		 */
		errMode: "alert",
	
	
		/**
		 * Feature plug-ins.
		 * 
		 * This is an array of objects which describe the feature plug-ins that are
		 * available to DataTables. These feature plug-ins are then available for
		 * use through the `dom` initialisation option.
		 * 
		 * Each feature plug-in is described by an object which must have the
		 * following properties:
		 * 
		 * * `fnInit` - function that is used to initialise the plug-in,
		 * * `cFeature` - a character so the feature can be enabled by the `dom`
		 *   instillation option. This is case sensitive.
		 *
		 * The `fnInit` function has the following input parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 *
		 * And the following return is expected:
		 * 
		 * * {node|null} The element which contains your feature. Note that the
		 *   return may also be void if your plug-in does not require to inject any
		 *   DOM elements into DataTables control (`dom`) - for example this might
		 *   be useful when developing a plug-in which allows table control via
		 *   keyboard entry
		 *
		 *  @type array
		 *
		 *  @example
		 *    $.fn.dataTable.ext.features.push( {
		 *      "fnInit": function( oSettings ) {
		 *        return new TableTools( { "oDTSettings": oSettings } );
		 *      },
		 *      "cFeature": "T"
		 *    } );
		 */
		feature: [],
	
	
		/**
		 * Row searching.
		 * 
		 * This method of searching is complimentary to the default type based
		 * searching, and a lot more comprehensive as it allows you complete control
		 * over the searching logic. Each element in this array is a function
		 * (parameters described below) that is called for every row in the table,
		 * and your logic decides if it should be included in the searching data set
		 * or not.
		 *
		 * Searching functions have the following input parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 * 2. `{array|object}` Data for the row to be processed (same as the
		 *    original format that was passed in as the data source, or an array
		 *    from a DOM data source
		 * 3. `{int}` Row index ({@link DataTable.models.oSettings.aoData}), which
		 *    can be useful to retrieve the `TR` element if you need DOM interaction.
		 *
		 * And the following return is expected:
		 *
		 * * {boolean} Include the row in the searched result set (true) or not
		 *   (false)
		 *
		 * Note that as with the main search ability in DataTables, technically this
		 * is "filtering", since it is subtractive. However, for consistency in
		 * naming we call it searching here.
		 *
		 *  @type array
		 *  @default []
		 *
		 *  @example
		 *    // The following example shows custom search being applied to the
		 *    // fourth column (i.e. the data[3] index) based on two input values
		 *    // from the end-user, matching the data in a certain range.
		 *    $.fn.dataTable.ext.search.push(
		 *      function( settings, data, dataIndex ) {
		 *        var min = document.getElementById('min').value * 1;
		 *        var max = document.getElementById('max').value * 1;
		 *        var version = data[3] == "-" ? 0 : data[3]*1;
		 *
		 *        if ( min == "" && max == "" ) {
		 *          return true;
		 *        }
		 *        else if ( min == "" && version < max ) {
		 *          return true;
		 *        }
		 *        else if ( min < version && "" == max ) {
		 *          return true;
		 *        }
		 *        else if ( min < version && version < max ) {
		 *          return true;
		 *        }
		 *        return false;
		 *      }
		 *    );
		 */
		search: [],
	
	
		/**
		 * Selector extensions
		 *
		 * The `selector` option can be used to extend the options available for the
		 * selector modifier options (`selector-modifier` object data type) that
		 * each of the three built in selector types offer (row, column and cell +
		 * their plural counterparts). For example the Select extension uses this
		 * mechanism to provide an option to select only rows, columns and cells
		 * that have been marked as selected by the end user (`{selected: true}`),
		 * which can be used in conjunction with the existing built in selector
		 * options.
		 *
		 * Each property is an array to which functions can be pushed. The functions
		 * take three attributes:
		 *
		 * * Settings object for the host table
		 * * Options object (`selector-modifier` object type)
		 * * Array of selected item indexes
		 *
		 * The return is an array of the resulting item indexes after the custom
		 * selector has been applied.
		 *
		 *  @type object
		 */
		selector: {
			cell: [],
			column: [],
			row: []
		},
	
	
		/**
		 * Internal functions, exposed for used in plug-ins.
		 * 
		 * Please note that you should not need to use the internal methods for
		 * anything other than a plug-in (and even then, try to avoid if possible).
		 * The internal function may change between releases.
		 *
		 *  @type object
		 *  @default {}
		 */
		internal: {},
	
	
		/**
		 * Legacy configuration options. Enable and disable legacy options that
		 * are available in DataTables.
		 *
		 *  @type object
		 */
		legacy: {
			/**
			 * Enable / disable DataTables 1.9 compatible server-side processing
			 * requests
			 *
			 *  @type boolean
			 *  @default null
			 */
			ajax: null
		},
	
	
		/**
		 * Pagination plug-in methods.
		 * 
		 * Each entry in this object is a function and defines which buttons should
		 * be shown by the pagination rendering method that is used for the table:
		 * {@link DataTable.ext.renderer.pageButton}. The renderer addresses how the
		 * buttons are displayed in the document, while the functions here tell it
		 * what buttons to display. This is done by returning an array of button
		 * descriptions (what each button will do).
		 *
		 * Pagination types (the four built in options and any additional plug-in
		 * options defined here) can be used through the `paginationType`
		 * initialisation parameter.
		 *
		 * The functions defined take two parameters:
		 *
		 * 1. `{int} page` The current page index
		 * 2. `{int} pages` The number of pages in the table
		 *
		 * Each function is expected to return an array where each element of the
		 * array can be one of:
		 *
		 * * `first` - Jump to first page when activated
		 * * `last` - Jump to last page when activated
		 * * `previous` - Show previous page when activated
		 * * `next` - Show next page when activated
		 * * `{int}` - Show page of the index given
		 * * `{array}` - A nested array containing the above elements to add a
		 *   containing 'DIV' element (might be useful for styling).
		 *
		 * Note that DataTables v1.9- used this object slightly differently whereby
		 * an object with two functions would be defined for each plug-in. That
		 * ability is still supported by DataTables 1.10+ to provide backwards
		 * compatibility, but this option of use is now decremented and no longer
		 * documented in DataTables 1.10+.
		 *
		 *  @type object
		 *  @default {}
		 *
		 *  @example
		 *    // Show previous, next and current page buttons only
		 *    $.fn.dataTableExt.oPagination.current = function ( page, pages ) {
		 *      return [ 'previous', page, 'next' ];
		 *    };
		 */
		pager: {},
	
	
		renderer: {
			pageButton: {},
			header: {}
		},
	
	
		/**
		 * Ordering plug-ins - custom data source
		 * 
		 * The extension options for ordering of data available here is complimentary
		 * to the default type based ordering that DataTables typically uses. It
		 * allows much greater control over the the data that is being used to
		 * order a column, but is necessarily therefore more complex.
		 * 
		 * This type of ordering is useful if you want to do ordering based on data
		 * live from the DOM (for example the contents of an 'input' element) rather
		 * than just the static string that DataTables knows of.
		 * 
		 * The way these plug-ins work is that you create an array of the values you
		 * wish to be ordering for the column in question and then return that
		 * array. The data in the array much be in the index order of the rows in
		 * the table (not the currently ordering order!). Which order data gathering
		 * function is run here depends on the `dt-init columns.orderDataType`
		 * parameter that is used for the column (if any).
		 *
		 * The functions defined take two parameters:
		 *
		 * 1. `{object}` DataTables settings object: see
		 *    {@link DataTable.models.oSettings}
		 * 2. `{int}` Target column index
		 *
		 * Each function is expected to return an array:
		 *
		 * * `{array}` Data for the column to be ordering upon
		 *
		 *  @type array
		 *
		 *  @example
		 *    // Ordering using `input` node values
		 *    $.fn.dataTable.ext.order['dom-text'] = function  ( settings, col )
		 *    {
		 *      return this.api().column( col, {order:'index'} ).nodes().map( function ( td, i ) {
		 *        return $('input', td).val();
		 *      } );
		 *    }
		 */
		order: {},
	
	
		/**
		 * Type based plug-ins.
		 *
		 * Each column in DataTables has a type assigned to it, either by automatic
		 * detection or by direct assignment using the `type` option for the column.
		 * The type of a column will effect how it is ordering and search (plug-ins
		 * can also make use of the column type if required).
		 *
		 * @namespace
		 */
		type: {
			/**
			 * Type detection functions.
			 *
			 * The functions defined in this object are used to automatically detect
			 * a column's type, making initialisation of DataTables super easy, even
			 * when complex data is in the table.
			 *
			 * The functions defined take two parameters:
			 *
		     *  1. `{*}` Data from the column cell to be analysed
		     *  2. `{settings}` DataTables settings object. This can be used to
		     *     perform context specific type detection - for example detection
		     *     based on language settings such as using a comma for a decimal
		     *     place. Generally speaking the options from the settings will not
		     *     be required
			 *
			 * Each function is expected to return:
			 *
			 * * `{string|null}` Data type detected, or null if unknown (and thus
			 *   pass it on to the other type detection functions.
			 *
			 *  @type array
			 *
			 *  @example
			 *    // Currency type detection plug-in:
			 *    $.fn.dataTable.ext.type.detect.push(
			 *      function ( data, settings ) {
			 *        // Check the numeric part
			 *        if ( ! $.isNumeric( data.substring(1) ) ) {
			 *          return null;
			 *        }
			 *
			 *        // Check prefixed by currency
			 *        if ( data.charAt(0) == '$' || data.charAt(0) == '&pound;' ) {
			 *          return 'currency';
			 *        }
			 *        return null;
			 *      }
			 *    );
			 */
			detect: [],
	
	
			/**
			 * Type based search formatting.
			 *
			 * The type based searching functions can be used to pre-format the
			 * data to be search on. For example, it can be used to strip HTML
			 * tags or to de-format telephone numbers for numeric only searching.
			 *
			 * Note that is a search is not defined for a column of a given type,
			 * no search formatting will be performed.
			 * 
			 * Pre-processing of searching data plug-ins - When you assign the sType
			 * for a column (or have it automatically detected for you by DataTables
			 * or a type detection plug-in), you will typically be using this for
			 * custom sorting, but it can also be used to provide custom searching
			 * by allowing you to pre-processing the data and returning the data in
			 * the format that should be searched upon. This is done by adding
			 * functions this object with a parameter name which matches the sType
			 * for that target column. This is the corollary of <i>afnSortData</i>
			 * for searching data.
			 *
			 * The functions defined take a single parameter:
			 *
		     *  1. `{*}` Data from the column cell to be prepared for searching
			 *
			 * Each function is expected to return:
			 *
			 * * `{string|null}` Formatted string that will be used for the searching.
			 *
			 *  @type object
			 *  @default {}
			 *
			 *  @example
			 *    $.fn.dataTable.ext.type.search['title-numeric'] = function ( d ) {
			 *      return d.replace(/\n/g," ").replace( /<.*?>/g, "" );
			 *    }
			 */
			search: {},
	
	
			/**
			 * Type based ordering.
			 *
			 * The column type tells DataTables what ordering to apply to the table
			 * when a column is sorted upon. The order for each type that is defined,
			 * is defined by the functions available in this object.
			 *
			 * Each ordering option can be described by three properties added to
			 * this object:
			 *
			 * * `{type}-pre` - Pre-formatting function
			 * * `{type}-asc` - Ascending order function
			 * * `{type}-desc` - Descending order function
			 *
			 * All three can be used together, only `{type}-pre` or only
			 * `{type}-asc` and `{type}-desc` together. It is generally recommended
			 * that only `{type}-pre` is used, as this provides the optimal
			 * implementation in terms of speed, although the others are provided
			 * for compatibility with existing Javascript sort functions.
			 *
			 * `{type}-pre`: Functions defined take a single parameter:
			 *
		     *  1. `{*}` Data from the column cell to be prepared for ordering
			 *
			 * And return:
			 *
			 * * `{*}` Data to be sorted upon
			 *
			 * `{type}-asc` and `{type}-desc`: Functions are typical Javascript sort
			 * functions, taking two parameters:
			 *
		     *  1. `{*}` Data to compare to the second parameter
		     *  2. `{*}` Data to compare to the first parameter
			 *
			 * And returning:
			 *
			 * * `{*}` Ordering match: <0 if first parameter should be sorted lower
			 *   than the second parameter, ===0 if the two parameters are equal and
			 *   >0 if the first parameter should be sorted height than the second
			 *   parameter.
			 * 
			 *  @type object
			 *  @default {}
			 *
			 *  @example
			 *    // Numeric ordering of formatted numbers with a pre-formatter
			 *    $.extend( $.fn.dataTable.ext.type.order, {
			 *      "string-pre": function(x) {
			 *        a = (a === "-" || a === "") ? 0 : a.replace( /[^\d\-\.]/g, "" );
			 *        return parseFloat( a );
			 *      }
			 *    } );
			 *
			 *  @example
			 *    // Case-sensitive string ordering, with no pre-formatting method
			 *    $.extend( $.fn.dataTable.ext.order, {
			 *      "string-case-asc": function(x,y) {
			 *        return ((x < y) ? -1 : ((x > y) ? 1 : 0));
			 *      },
			 *      "string-case-desc": function(x,y) {
			 *        return ((x < y) ? 1 : ((x > y) ? -1 : 0));
			 *      }
			 *    } );
			 */
			order: {}
		},
	
		/**
		 * Unique DataTables instance counter
		 *
		 * @type int
		 * @private
		 */
		_unique: 0,
	
	
		//
		// Depreciated
		// The following properties are retained for backwards compatiblity only.
		// The should not be used in new projects and will be removed in a future
		// version
		//
	
		/**
		 * Version check function.
		 *  @type function
		 *  @depreciated Since 1.10
		 */
		fnVersionCheck: DataTable.fnVersionCheck,
	
	
		/**
		 * Index for what 'this' index API functions should use
		 *  @type int
		 *  @deprecated Since v1.10
		 */
		iApiIndex: 0,
	
	
		/**
		 * jQuery UI class container
		 *  @type object
		 *  @deprecated Since v1.10
		 */
		oJUIClasses: {},
	
	
		/**
		 * Software version
		 *  @type string
		 *  @deprecated Since v1.10
		 */
		sVersion: DataTable.version
	};
	
	
	//
	// Backwards compatibility. Alias to pre 1.10 Hungarian notation counter parts
	//
	$.extend( _ext, {
		afnFiltering: _ext.search,
		aTypes:       _ext.type.detect,
		ofnSearch:    _ext.type.search,
		oSort:        _ext.type.order,
		afnSortData:  _ext.order,
		aoFeatures:   _ext.feature,
		oApi:         _ext.internal,
		oStdClasses:  _ext.classes,
		oPagination:  _ext.pager
	} );
	
	
	$.extend( DataTable.ext.classes, {
		"sTable": "dataTable",
		"sNoFooter": "no-footer",
	
		/* Paging buttons */
		"sPageButton": "paginate_button",
		"sPageButtonActive": "current",
		"sPageButtonDisabled": "disabled",
	
		/* Striping classes */
		"sStripeOdd": "odd",
		"sStripeEven": "even",
	
		/* Empty row */
		"sRowEmpty": "dataTables_empty",
	
		/* Features */
		"sWrapper": "dataTables_wrapper",
		"sFilter": "dataTables_filter",
		"sInfo": "dataTables_info",
		"sPaging": "dataTables_paginate paging_", /* Note that the type is postfixed */
		"sLength": "dataTables_length",
		"sProcessing": "dataTables_processing",
	
		/* Sorting */
		"sSortAsc": "sorting_asc",
		"sSortDesc": "sorting_desc",
		"sSortable": "sorting", /* Sortable in both directions */
		"sSortableAsc": "sorting_asc_disabled",
		"sSortableDesc": "sorting_desc_disabled",
		"sSortableNone": "sorting_disabled",
		"sSortColumn": "sorting_", /* Note that an int is postfixed for the sorting order */
	
		/* Filtering */
		"sFilterInput": "",
	
		/* Page length */
		"sLengthSelect": "",
	
		/* Scrolling */
		"sScrollWrapper": "dataTables_scroll",
		"sScrollHead": "dataTables_scrollHead",
		"sScrollHeadInner": "dataTables_scrollHeadInner",
		"sScrollBody": "dataTables_scrollBody",
		"sScrollFoot": "dataTables_scrollFoot",
		"sScrollFootInner": "dataTables_scrollFootInner",
	
		/* Misc */
		"sHeaderTH": "",
		"sFooterTH": "",
	
		// Deprecated
		"sSortJUIAsc": "",
		"sSortJUIDesc": "",
		"sSortJUI": "",
		"sSortJUIAscAllowed": "",
		"sSortJUIDescAllowed": "",
		"sSortJUIWrapper": "",
		"sSortIcon": "",
		"sJUIHeader": "",
		"sJUIFooter": ""
	} );
	
	
	var extPagination = DataTable.ext.pager;
	
	function _numbers ( page, pages ) {
		var
			numbers = [],
			buttons = extPagination.numbers_length,
			half = Math.floor( buttons / 2 ),
			i = 1;
	
		if ( pages <= buttons ) {
			numbers = _range( 0, pages );
		}
		else if ( page <= half ) {
			numbers = _range( 0, buttons-2 );
			numbers.push( 'ellipsis' );
			numbers.push( pages-1 );
		}
		else if ( page >= pages - 1 - half ) {
			numbers = _range( pages-(buttons-2), pages );
			numbers.splice( 0, 0, 'ellipsis' ); // no unshift in ie6
			numbers.splice( 0, 0, 0 );
		}
		else {
			numbers = _range( page-half+2, page+half-1 );
			numbers.push( 'ellipsis' );
			numbers.push( pages-1 );
			numbers.splice( 0, 0, 'ellipsis' );
			numbers.splice( 0, 0, 0 );
		}
	
		numbers.DT_el = 'span';
		return numbers;
	}
	
	
	$.extend( extPagination, {
		simple: function ( page, pages ) {
			return [ 'previous', 'next' ];
		},
	
		full: function ( page, pages ) {
			return [  'first', 'previous', 'next', 'last' ];
		},
	
		numbers: function ( page, pages ) {
			return [ _numbers(page, pages) ];
		},
	
		simple_numbers: function ( page, pages ) {
			return [ 'previous', _numbers(page, pages), 'next' ];
		},
	
		full_numbers: function ( page, pages ) {
			return [ 'first', 'previous', _numbers(page, pages), 'next', 'last' ];
		},
		
		first_last_numbers: function (page, pages) {
	 		return ['first', _numbers(page, pages), 'last'];
	 	},
	
		// For testing and plug-ins to use
		_numbers: _numbers,
	
		// Number of number buttons (including ellipsis) to show. _Must be odd!_
		numbers_length: 7
	} );
	
	
	$.extend( true, DataTable.ext.renderer, {
		pageButton: {
			_: function ( settings, host, idx, buttons, page, pages ) {
				var classes = settings.oClasses;
				var lang = settings.oLanguage.oPaginate;
				var aria = settings.oLanguage.oAria.paginate || {};
				var btnDisplay, btnClass, counter=0;
	
				var attach = function( container, buttons ) {
					var i, ien, node, button;
					var clickHandler = function ( e ) {
						_fnPageChange( settings, e.data.action, true );
					};
	
					for ( i=0, ien=buttons.length ; i<ien ; i++ ) {
						button = buttons[i];
	
						if ( $.isArray( button ) ) {
							var inner = $( '<'+(button.DT_el || 'div')+'/>' )
								.appendTo( container );
							attach( inner, button );
						}
						else {
							btnDisplay = null;
							btnClass = '';
	
							switch ( button ) {
								case 'ellipsis':
									container.append('<span class="ellipsis">&#x2026;</span>');
									break;
	
								case 'first':
									btnDisplay = lang.sFirst;
									btnClass = button + (page > 0 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'previous':
									btnDisplay = lang.sPrevious;
									btnClass = button + (page > 0 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'next':
									btnDisplay = lang.sNext;
									btnClass = button + (page < pages-1 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								case 'last':
									btnDisplay = lang.sLast;
									btnClass = button + (page < pages-1 ?
										'' : ' '+classes.sPageButtonDisabled);
									break;
	
								default:
									btnDisplay = button + 1;
									btnClass = page === button ?
										classes.sPageButtonActive : '';
									break;
							}
	
							if ( btnDisplay !== null ) {
								node = $('<a>', {
										'class': classes.sPageButton+' '+btnClass,
										'aria-controls': settings.sTableId,
										'aria-label': aria[ button ],
										'data-dt-idx': counter,
										'tabindex': settings.iTabIndex,
										'id': idx === 0 && typeof button === 'string' ?
											settings.sTableId +'_'+ button :
											null
									} )
									.html( btnDisplay )
									.appendTo( container );
	
								_fnBindAction(
									node, {action: button}, clickHandler
								);
	
								counter++;
							}
						}
					}
				};
	
				// IE9 throws an 'unknown error' if document.activeElement is used
				// inside an iframe or frame. Try / catch the error. Not good for
				// accessibility, but neither are frames.
				var activeEl;
	
				try {
					// Because this approach is destroying and recreating the paging
					// elements, focus is lost on the select button which is bad for
					// accessibility. So we want to restore focus once the draw has
					// completed
					activeEl = $(host).find(document.activeElement).data('dt-idx');
				}
				catch (e) {}
	
				attach( $(host).empty(), buttons );
	
				if ( activeEl !== undefined ) {
					$(host).find( '[data-dt-idx='+activeEl+']' ).focus();
				}
			}
		}
	} );
	
	
	
	// Built in type detection. See model.ext.aTypes for information about
	// what is required from this methods.
	$.extend( DataTable.ext.type.detect, [
		// Plain numbers - first since V8 detects some plain numbers as dates
		// e.g. Date.parse('55') (but not all, e.g. Date.parse('22')...).
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _isNumber( d, decimal ) ? 'num'+decimal : null;
		},
	
		// Dates (only those recognised by the browser's Date.parse)
		function ( d, settings )
		{
			// V8 tries _very_ hard to make a string passed into `Date.parse()`
			// valid, so we need to use a regex to restrict date formats. Use a
			// plug-in for anything other than ISO8601 style strings
			if ( d && !(d instanceof Date) && ! _re_date.test(d) ) {
				return null;
			}
			var parsed = Date.parse(d);
			return (parsed !== null && !isNaN(parsed)) || _empty(d) ? 'date' : null;
		},
	
		// Formatted numbers
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _isNumber( d, decimal, true ) ? 'num-fmt'+decimal : null;
		},
	
		// HTML numeric
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _htmlNumeric( d, decimal ) ? 'html-num'+decimal : null;
		},
	
		// HTML numeric, formatted
		function ( d, settings )
		{
			var decimal = settings.oLanguage.sDecimal;
			return _htmlNumeric( d, decimal, true ) ? 'html-num-fmt'+decimal : null;
		},
	
		// HTML (this is strict checking - there must be html)
		function ( d, settings )
		{
			return _empty( d ) || (typeof d === 'string' && d.indexOf('<') !== -1) ?
				'html' : null;
		}
	] );
	
	
	
	// Filter formatting functions. See model.ext.ofnSearch for information about
	// what is required from these methods.
	// 
	// Note that additional search methods are added for the html numbers and
	// html formatted numbers by `_addNumericSort()` when we know what the decimal
	// place is
	
	
	$.extend( DataTable.ext.type.search, {
		html: function ( data ) {
			return _empty(data) ?
				data :
				typeof data === 'string' ?
					data
						.replace( _re_new_lines, " " )
						.replace( _re_html, "" ) :
					'';
		},
	
		string: function ( data ) {
			return _empty(data) ?
				data :
				typeof data === 'string' ?
					data.replace( _re_new_lines, " " ) :
					data;
		}
	} );
	
	
	
	var __numericReplace = function ( d, decimalPlace, re1, re2 ) {
		if ( d !== 0 && (!d || d === '-') ) {
			return -Infinity;
		}
	
		// If a decimal place other than `.` is used, it needs to be given to the
		// function so we can detect it and replace with a `.` which is the only
		// decimal place Javascript recognises - it is not locale aware.
		if ( decimalPlace ) {
			d = _numToDecimal( d, decimalPlace );
		}
	
		if ( d.replace ) {
			if ( re1 ) {
				d = d.replace( re1, '' );
			}
	
			if ( re2 ) {
				d = d.replace( re2, '' );
			}
		}
	
		return d * 1;
	};
	
	
	// Add the numeric 'deformatting' functions for sorting and search. This is done
	// in a function to provide an easy ability for the language options to add
	// additional methods if a non-period decimal place is used.
	function _addNumericSort ( decimalPlace ) {
		$.each(
			{
				// Plain numbers
				"num": function ( d ) {
					return __numericReplace( d, decimalPlace );
				},
	
				// Formatted numbers
				"num-fmt": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_formatted_numeric );
				},
	
				// HTML numeric
				"html-num": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_html );
				},
	
				// HTML numeric, formatted
				"html-num-fmt": function ( d ) {
					return __numericReplace( d, decimalPlace, _re_html, _re_formatted_numeric );
				}
			},
			function ( key, fn ) {
				// Add the ordering method
				_ext.type.order[ key+decimalPlace+'-pre' ] = fn;
	
				// For HTML types add a search formatter that will strip the HTML
				if ( key.match(/^html\-/) ) {
					_ext.type.search[ key+decimalPlace ] = _ext.type.search.html;
				}
			}
		);
	}
	
	
	// Default sort methods
	$.extend( _ext.type.order, {
		// Dates
		"date-pre": function ( d ) {
			return Date.parse( d ) || -Infinity;
		},
	
		// html
		"html-pre": function ( a ) {
			return _empty(a) ?
				'' :
				a.replace ?
					a.replace( /<.*?>/g, "" ).toLowerCase() :
					a+'';
		},
	
		// string
		"string-pre": function ( a ) {
			// This is a little complex, but faster than always calling toString,
			// http://jsperf.com/tostring-v-check
			return _empty(a) ?
				'' :
				typeof a === 'string' ?
					a.toLowerCase() :
					! a.toString ?
						'' :
						a.toString();
		},
	
		// string-asc and -desc are retained only for compatibility with the old
		// sort methods
		"string-asc": function ( x, y ) {
			return ((x < y) ? -1 : ((x > y) ? 1 : 0));
		},
	
		"string-desc": function ( x, y ) {
			return ((x < y) ? 1 : ((x > y) ? -1 : 0));
		}
	} );
	
	
	// Numeric sorting types - order doesn't matter here
	_addNumericSort( '' );
	
	
	$.extend( true, DataTable.ext.renderer, {
		header: {
			_: function ( settings, cell, column, classes ) {
				// No additional mark-up required
				// Attach a sort listener to update on sort - note that using the
				// `DT` namespace will allow the event to be removed automatically
				// on destroy, while the `dt` namespaced event is the one we are
				// listening for
				$(settings.nTable).on( 'order.dt.DT', function ( e, ctx, sorting, columns ) {
					if ( settings !== ctx ) { // need to check this this is the host
						return;               // table, not a nested one
					}
	
					var colIdx = column.idx;
	
					cell
						.removeClass(
							column.sSortingClass +' '+
							classes.sSortAsc +' '+
							classes.sSortDesc
						)
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortDesc :
								column.sSortingClass
						);
				} );
			},
	
			jqueryui: function ( settings, cell, column, classes ) {
				$('<div/>')
					.addClass( classes.sSortJUIWrapper )
					.append( cell.contents() )
					.append( $('<span/>')
						.addClass( classes.sSortIcon+' '+column.sSortingClassJUI )
					)
					.appendTo( cell );
	
				// Attach a sort listener to update on sort
				$(settings.nTable).on( 'order.dt.DT', function ( e, ctx, sorting, columns ) {
					if ( settings !== ctx ) {
						return;
					}
	
					var colIdx = column.idx;
	
					cell
						.removeClass( classes.sSortAsc +" "+classes.sSortDesc )
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortDesc :
								column.sSortingClass
						);
	
					cell
						.find( 'span.'+classes.sSortIcon )
						.removeClass(
							classes.sSortJUIAsc +" "+
							classes.sSortJUIDesc +" "+
							classes.sSortJUI +" "+
							classes.sSortJUIAscAllowed +" "+
							classes.sSortJUIDescAllowed
						)
						.addClass( columns[ colIdx ] == 'asc' ?
							classes.sSortJUIAsc : columns[ colIdx ] == 'desc' ?
								classes.sSortJUIDesc :
								column.sSortingClassJUI
						);
				} );
			}
		}
	} );
	
	/*
	 * Public helper functions. These aren't used internally by DataTables, or
	 * called by any of the options passed into DataTables, but they can be used
	 * externally by developers working with DataTables. They are helper functions
	 * to make working with DataTables a little bit easier.
	 */
	
	var __htmlEscapeEntities = function ( d ) {
		return typeof d === 'string' ?
			d.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;') :
			d;
	};
	
	/**
	 * Helpers for `columns.render`.
	 *
	 * The options defined here can be used with the `columns.render` initialisation
	 * option to provide a display renderer. The following functions are defined:
	 *
	 * * `number` - Will format numeric data (defined by `columns.data`) for
	 *   display, retaining the original unformatted data for sorting and filtering.
	 *   It takes 5 parameters:
	 *   * `string` - Thousands grouping separator
	 *   * `string` - Decimal point indicator
	 *   * `integer` - Number of decimal points to show
	 *   * `string` (optional) - Prefix.
	 *   * `string` (optional) - Postfix (/suffix).
	 * * `text` - Escape HTML to help prevent XSS attacks. It has no optional
	 *   parameters.
	 *
	 * @example
	 *   // Column definition using the number renderer
	 *   {
	 *     data: "salary",
	 *     render: $.fn.dataTable.render.number( '\'', '.', 0, '$' )
	 *   }
	 *
	 * @namespace
	 */
	DataTable.render = {
		number: function ( thousands, decimal, precision, prefix, postfix ) {
			return {
				display: function ( d ) {
					if ( typeof d !== 'number' && typeof d !== 'string' ) {
						return d;
					}
	
					var negative = d < 0 ? '-' : '';
					var flo = parseFloat( d );
	
					// If NaN then there isn't much formatting that we can do - just
					// return immediately, escaping any HTML (this was supposed to
					// be a number after all)
					if ( isNaN( flo ) ) {
						return __htmlEscapeEntities( d );
					}
	
					flo = flo.toFixed( precision );
					d = Math.abs( flo );
	
					var intPart = parseInt( d, 10 );
					var floatPart = precision ?
						decimal+(d - intPart).toFixed( precision ).substring( 2 ):
						'';
	
					return negative + (prefix||'') +
						intPart.toString().replace(
							/\B(?=(\d{3})+(?!\d))/g, thousands
						) +
						floatPart +
						(postfix||'');
				}
			};
		},
	
		text: function () {
			return {
				display: __htmlEscapeEntities
			};
		}
	};
	
	
	/*
	 * This is really a good bit rubbish this method of exposing the internal methods
	 * publicly... - To be fixed in 2.0 using methods on the prototype
	 */
	
	
	/**
	 * Create a wrapper function for exporting an internal functions to an external API.
	 *  @param {string} fn API function name
	 *  @returns {function} wrapped function
	 *  @memberof DataTable#internal
	 */
	function _fnExternApiFunc (fn)
	{
		return function() {
			var args = [_fnSettingsFromNode( this[DataTable.ext.iApiIndex] )].concat(
				Array.prototype.slice.call(arguments)
			);
			return DataTable.ext.internal[fn].apply( this, args );
		};
	}
	
	
	/**
	 * Reference to internal functions for use by plug-in developers. Note that
	 * these methods are references to internal functions and are considered to be
	 * private. If you use these methods, be aware that they are liable to change
	 * between versions.
	 *  @namespace
	 */
	$.extend( DataTable.ext.internal, {
		_fnExternApiFunc: _fnExternApiFunc,
		_fnBuildAjax: _fnBuildAjax,
		_fnAjaxUpdate: _fnAjaxUpdate,
		_fnAjaxParameters: _fnAjaxParameters,
		_fnAjaxUpdateDraw: _fnAjaxUpdateDraw,
		_fnAjaxDataSrc: _fnAjaxDataSrc,
		_fnAddColumn: _fnAddColumn,
		_fnColumnOptions: _fnColumnOptions,
		_fnAdjustColumnSizing: _fnAdjustColumnSizing,
		_fnVisibleToColumnIndex: _fnVisibleToColumnIndex,
		_fnColumnIndexToVisible: _fnColumnIndexToVisible,
		_fnVisbleColumns: _fnVisbleColumns,
		_fnGetColumns: _fnGetColumns,
		_fnColumnTypes: _fnColumnTypes,
		_fnApplyColumnDefs: _fnApplyColumnDefs,
		_fnHungarianMap: _fnHungarianMap,
		_fnCamelToHungarian: _fnCamelToHungarian,
		_fnLanguageCompat: _fnLanguageCompat,
		_fnBrowserDetect: _fnBrowserDetect,
		_fnAddData: _fnAddData,
		_fnAddTr: _fnAddTr,
		_fnNodeToDataIndex: _fnNodeToDataIndex,
		_fnNodeToColumnIndex: _fnNodeToColumnIndex,
		_fnGetCellData: _fnGetCellData,
		_fnSetCellData: _fnSetCellData,
		_fnSplitObjNotation: _fnSplitObjNotation,
		_fnGetObjectDataFn: _fnGetObjectDataFn,
		_fnSetObjectDataFn: _fnSetObjectDataFn,
		_fnGetDataMaster: _fnGetDataMaster,
		_fnClearTable: _fnClearTable,
		_fnDeleteIndex: _fnDeleteIndex,
		_fnInvalidate: _fnInvalidate,
		_fnGetRowElements: _fnGetRowElements,
		_fnCreateTr: _fnCreateTr,
		_fnBuildHead: _fnBuildHead,
		_fnDrawHead: _fnDrawHead,
		_fnDraw: _fnDraw,
		_fnReDraw: _fnReDraw,
		_fnAddOptionsHtml: _fnAddOptionsHtml,
		_fnDetectHeader: _fnDetectHeader,
		_fnGetUniqueThs: _fnGetUniqueThs,
		_fnFeatureHtmlFilter: _fnFeatureHtmlFilter,
		_fnFilterComplete: _fnFilterComplete,
		_fnFilterCustom: _fnFilterCustom,
		_fnFilterColumn: _fnFilterColumn,
		_fnFilter: _fnFilter,
		_fnFilterCreateSearch: _fnFilterCreateSearch,
		_fnEscapeRegex: _fnEscapeRegex,
		_fnFilterData: _fnFilterData,
		_fnFeatureHtmlInfo: _fnFeatureHtmlInfo,
		_fnUpdateInfo: _fnUpdateInfo,
		_fnInfoMacros: _fnInfoMacros,
		_fnInitialise: _fnInitialise,
		_fnInitComplete: _fnInitComplete,
		_fnLengthChange: _fnLengthChange,
		_fnFeatureHtmlLength: _fnFeatureHtmlLength,
		_fnFeatureHtmlPaginate: _fnFeatureHtmlPaginate,
		_fnPageChange: _fnPageChange,
		_fnFeatureHtmlProcessing: _fnFeatureHtmlProcessing,
		_fnProcessingDisplay: _fnProcessingDisplay,
		_fnFeatureHtmlTable: _fnFeatureHtmlTable,
		_fnScrollDraw: _fnScrollDraw,
		_fnApplyToChildren: _fnApplyToChildren,
		_fnCalculateColumnWidths: _fnCalculateColumnWidths,
		_fnThrottle: _fnThrottle,
		_fnConvertToWidth: _fnConvertToWidth,
		_fnGetWidestNode: _fnGetWidestNode,
		_fnGetMaxLenString: _fnGetMaxLenString,
		_fnStringToCss: _fnStringToCss,
		_fnSortFlatten: _fnSortFlatten,
		_fnSort: _fnSort,
		_fnSortAria: _fnSortAria,
		_fnSortListener: _fnSortListener,
		_fnSortAttachListener: _fnSortAttachListener,
		_fnSortingClasses: _fnSortingClasses,
		_fnSortData: _fnSortData,
		_fnSaveState: _fnSaveState,
		_fnLoadState: _fnLoadState,
		_fnSettingsFromNode: _fnSettingsFromNode,
		_fnLog: _fnLog,
		_fnMap: _fnMap,
		_fnBindAction: _fnBindAction,
		_fnCallbackReg: _fnCallbackReg,
		_fnCallbackFire: _fnCallbackFire,
		_fnLengthOverflow: _fnLengthOverflow,
		_fnRenderer: _fnRenderer,
		_fnDataSource: _fnDataSource,
		_fnRowAttributes: _fnRowAttributes,
		_fnCalculateEnd: function () {} // Used by a lot of plug-ins, but redundant
		                                // in 1.10, so this dead-end function is
		                                // added to prevent errors
	} );
	

	// jQuery access
	$.fn.dataTable = DataTable;

	// Provide access to the host jQuery object (circular reference)
	DataTable.$ = $;

	// Legacy aliases
	$.fn.dataTableSettings = DataTable.settings;
	$.fn.dataTableExt = DataTable.ext;

	// With a capital `D` we return a DataTables API instance rather than a
	// jQuery object
	$.fn.DataTable = function ( opts ) {
		return $(this).dataTable( opts ).api();
	};

	// All properties that are available to $.fn.dataTable should also be
	// available on $.fn.DataTable
	$.each( DataTable, function ( prop, val ) {
		$.fn.DataTable[ prop ] = val;
	} );


	// Information about events fired by DataTables - for documentation.
	/**
	 * Draw event, fired whenever the table is redrawn on the page, at the same
	 * point as fnDrawCallback. This may be useful for binding events or
	 * performing calculations when the table is altered at all.
	 *  @name DataTable#draw.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Search event, fired when the searching applied to the table (using the
	 * built-in global search, or column filters) is altered.
	 *  @name DataTable#search.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Page change event, fired when the paging of the table is altered.
	 *  @name DataTable#page.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Order event, fired when the ordering applied to the table is altered.
	 *  @name DataTable#order.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * DataTables initialisation complete event, fired when the table is fully
	 * drawn, including Ajax data loaded, if Ajax data is required.
	 *  @name DataTable#init.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The JSON object request from the server - only
	 *    present if client-side Ajax sourced data is used</li></ol>
	 */

	/**
	 * State save event, fired when the table has changed state a new state save
	 * is required. This event allows modification of the state saving object
	 * prior to actually doing the save, including addition or other state
	 * properties (for plug-ins) or modification of a DataTables core property.
	 *  @name DataTable#stateSaveParams.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The state information to be saved
	 */

	/**
	 * State load event, fired when the table is loading state from the stored
	 * data, but prior to the settings object being modified by the saved state
	 * - allowing modification of the saved state is required or loading of
	 * state for a plug-in.
	 *  @name DataTable#stateLoadParams.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The saved state information
	 */

	/**
	 * State loaded event, fired when state has been loaded from stored data and
	 * the settings object has been modified by the loaded data.
	 *  @name DataTable#stateLoaded.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {object} json The saved state information
	 */

	/**
	 * Processing event, fired when DataTables is doing some kind of processing
	 * (be it, order, searcg or anything else). It can be used to indicate to
	 * the end user that there is something happening, or that something has
	 * finished.
	 *  @name DataTable#processing.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} oSettings DataTables settings object
	 *  @param {boolean} bShow Flag for if DataTables is doing processing or not
	 */

	/**
	 * Ajax (XHR) event, fired whenever an Ajax request is completed from a
	 * request to made to the server for new data. This event is called before
	 * DataTables processed the returned data, so it can also be used to pre-
	 * process the data returned from the server, if needed.
	 *
	 * Note that this trigger is called in `fnServerData`, if you override
	 * `fnServerData` and which to use this event, you need to trigger it in you
	 * success function.
	 *  @name DataTable#xhr.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {object} json JSON returned from the server
	 *
	 *  @example
	 *     // Use a custom property returned from the server in another DOM element
	 *     $('#table').dataTable().on('xhr.dt', function (e, settings, json) {
	 *       $('#status').html( json.status );
	 *     } );
	 *
	 *  @example
	 *     // Pre-process the data returned from the server
	 *     $('#table').dataTable().on('xhr.dt', function (e, settings, json) {
	 *       for ( var i=0, ien=json.aaData.length ; i<ien ; i++ ) {
	 *         json.aaData[i].sum = json.aaData[i].one + json.aaData[i].two;
	 *       }
	 *       // Note no return - manipulate the data directly in the JSON object.
	 *     } );
	 */

	/**
	 * Destroy event, fired when the DataTable is destroyed by calling fnDestroy
	 * or passing the bDestroy:true parameter in the initialisation object. This
	 * can be used to remove bound events, added DOM nodes, etc.
	 *  @name DataTable#destroy.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Page length change event, fired when number of records to show on each
	 * page (the length) is changed.
	 *  @name DataTable#length.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {integer} len New length
	 */

	/**
	 * Column sizing has changed.
	 *  @name DataTable#column-sizing.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 */

	/**
	 * Column visibility has changed.
	 *  @name DataTable#column-visibility.dt
	 *  @event
	 *  @param {event} e jQuery event object
	 *  @param {object} o DataTables settings object {@link DataTable.models.oSettings}
	 *  @param {int} column Column index
	 *  @param {bool} vis `false` if column now hidden, or `true` if visible
	 */

	return $.fn.dataTable;
}));


/*! DataTables Editor v1.7.0
 *
 * ©2012-2017 SpryMedia Ltd, all rights reserved.
 * License: editor.datatables.net/license
 */

/**
 * @summary     DataTables Editor
 * @description Table editing library for DataTables
 * @version     1.7.0
 * @file        dataTables.editor.js
 * @author      SpryMedia Ltd
 * @contact     www.datatables.net/contact
 */

/*jslint evil: true, undef: true, browser: true */
/*globals jQuery,alert,console */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


if ( ! DataTable || ! DataTable.versionCheck || ! DataTable.versionCheck('1.10.7') ) {
	throw 'Editor requires DataTables 1.10.7 or newer';
}

/**
 * Editor is a plug-in for <a href="http://datatables.net">DataTables</a> which
 * provides an interface for creating, reading, editing and deleting and entries
 * (a CRUD interface) in a DataTable. The documentation presented here is
 * primarily focused on presenting the API for Editor. For a full list of
 * features, examples and the server interface protocol, please refer to the <a
 * href="http://editor.datatables.net">Editor web-site</a>.
 *
 * Note that in this documentation, for brevity, the `DataTable` refers to the
 * jQuery parameter `jQuery.fn.dataTable` through which it may be  accessed.
 * Therefore, when creating a new Editor instance, use `jQuery.fn.Editor` as
 * shown in the examples below.
 *
 *  @class
 *  @param {object} [oInit={}] Configuration object for Editor. Options
 *    are defined by {@link Editor.defaults}.
 *  @requires jQuery 1.7+
 *  @requires DataTables 1.10+
 */
var Editor = function ( opts )
{
	if ( ! (this instanceof Editor) ) {
		alert( "DataTables Editor must be initialised as a 'new' instance'" );
	}

	this._constructor( opts );
};

// Export Editor as a DataTables property
DataTable.Editor = Editor;
$.fn.DataTable.Editor = Editor;

// Internal methods


/**
 * Get an Editor node based on the data-dte-e (element) attribute and return it
 * as a jQuery object.
 *  @param {string} dis The data-dte-e attribute name to match for the element
 *  @param {node} [ctx=document] The context for the search - recommended this
 *    parameter is included for performance.
 *  @returns {jQuery} jQuery object of found node(s).
 *  @private
 */
var _editor_el = function ( dis, ctx )
{
	if ( ctx === undefined ) {
		ctx = document;
	}

	return $('*[data-dte-e="'+dis+'"]', ctx);
};


/** @internal Counter for unique event namespaces in the inline control */
var __inlineCounter = 0;

var _pluck = function ( a, prop )
{
	var out = [];

	$.each( a, function ( idx, el ) {
		out.push( el[ prop ] );
	} );

	return out;
};

// The file and file methods are common on both the DataTables and Editor APIs
// so rather than writing the same methods twice, they are defined once here and
// assigned as required.
var _api_file = function ( name, id )
{
	var table = this.files( name ); // can throw. `this` will be Editor or
	var file = table[ id ];         //  DataTables.Api context. Both work.

	if ( ! file ) {
		throw 'Unknown file id '+ id +' in table '+ name;
	}

	return table[ id ];
};

var _api_files = function ( name )
{
	if ( ! name ) {
		return Editor.files;
	}

	var table = Editor.files[ name ];

	if ( ! table ) {
		throw 'Unknown file table name: '+ name;
	}

	return table;
};

/**
 * Get the keys of an object / array
 *
 * @param  {object} o Object to get the keys of
 * @return {array} Keys
 */
var _objectKeys = function ( o ) {
	var out = [];

	for ( var key in o ) {
		if ( o.hasOwnProperty( key ) ) {
			out.push( key );
		}
	}

	return out;
};

/**
 * Compare parameters for difference - diving into arrays and objects if
 * needed, allowing the object reference to be different, but the contents to
 * match.
 *
 * Please note that LOOSE type checking is used
 *
 * @param  {*} o1 Object to compare
 * @param  {*} o2 Object to compare
 * @return {boolean} `true` if matching, `false` otherwise
 */
var _deepCompare = function (o1, o2) {
	if ( typeof o1 !== 'object' || typeof o2 !== 'object' ) {
		return o1 == o2;
	}

	var o1Props = _objectKeys( o1 );
	var o2Props = _objectKeys( o2 );

	if (o1Props.length !== o2Props.length) {
		return false;
	}

	for ( var i=0, ien=o1Props.length ; i<ien ; i++ ) {
		var propName = o1Props[i];

		if ( typeof o1[propName] === 'object' ) {
			if ( ! _deepCompare( o1[propName], o2[propName] ) ) {
				return false;
			}
		}
		else if (o1[propName] != o2[propName]) {
			return false;
		}
	}

	return true;
};

// Field class


Editor.Field = function ( opts, classes, host ) {
	var that = this;
	var multiI18n = host.i18n.multi;

	opts = $.extend( true, {}, Editor.Field.defaults, opts );
	
	if ( ! Editor.fieldTypes[ opts.type ] ) {
		throw "Error adding field - unknown field type "+opts.type;
	}

	this.s = $.extend( {}, Editor.Field.settings, { // has to be a shallow copy!
		type:       Editor.fieldTypes[ opts.type ],
		name:       opts.name,
		classes:    classes,
		host:       host,
		opts:       opts,
		multiValue: false
	} );

	// No id, so assign one to have the label reference work
	if ( ! opts.id ) {
		opts.id = 'DTE_Field_'+opts.name;
	}

	// Backwards compatibility
	if ( opts.dataProp ) {
		opts.data = opts.dataProp;
	}

	// If no `data` option is given, then we use the name from the field as the
	// data prop to read data for the field from DataTables
	if ( opts.data === '' ) {
		opts.data = opts.name;
	}

	// Get and set functions in the data object for the record
	var dtPrivateApi = DataTable.ext.oApi;
	this.valFromData = function ( d ) { // get val from data
		// wrapper to automatically pass `editor` as the type
		return dtPrivateApi._fnGetObjectDataFn( opts.data )( d, 'editor' );
	};
	this.valToData = dtPrivateApi._fnSetObjectDataFn( opts.data ); // set val to data

	// Field HTML structure
	var template = $(
		'<div class="'+classes.wrapper+' '+classes.typePrefix+opts.type+' '+classes.namePrefix+opts.name+' '+opts.className+'">'+
			'<label data-dte-e="label" class="'+classes.label+'" for="'+Editor.safeId(opts.id)+'">'+
				opts.label+
				'<div data-dte-e="msg-label" class="'+classes['msg-label']+'">'+opts.labelInfo+'</div>'+
			'</label>'+
			'<div data-dte-e="input" class="'+classes.input+'">'+
				// Field specific HTML is added here if there is any
				'<div data-dte-e="input-control" class="'+classes.inputControl+'"/>'+
				'<div data-dte-e="multi-value" class="'+classes.multiValue+'">'+
					multiI18n.title+
					'<span data-dte-e="multi-info" class="'+classes.multiInfo+'">'+
						multiI18n.info+
					'</span>'+
				'</div>'+
				'<div data-dte-e="msg-multi" class="'+classes.multiRestore+'">'+
					multiI18n.restore+
				'</div>'+
				'<div data-dte-e="msg-error" class="'+classes['msg-error']+'"></div>'+
				'<div data-dte-e="msg-message" class="'+classes['msg-message']+'">'+opts.message+'</div>'+
				'<div data-dte-e="msg-info" class="'+classes['msg-info']+'">'+opts.fieldInfo+'</div>'+
			'</div>'+
		'</div>');

	var input = this._typeFn( 'create', opts );
	if ( input !== null ) {
		_editor_el('input-control', template).prepend( input );
	}
	else {
		template.css('display', "none");
	}

	this.dom = $.extend( true, {}, Editor.Field.models.dom, {
		container:    template,
		inputControl: _editor_el('input-control', template),
		label:        _editor_el('label', template),
		fieldInfo:    _editor_el('msg-info', template),
		labelInfo:    _editor_el('msg-label', template),
		fieldError:   _editor_el('msg-error', template),
		fieldMessage: _editor_el('msg-message', template),
		multi:        _editor_el('multi-value', template),
		multiReturn:  _editor_el('msg-multi', template),
		multiInfo:    _editor_el('multi-info', template)
	} );

	// On click - set a common value for the field
	this.dom.multi.on( 'click', function () {
		if ( that.s.opts.multiEditable && ! template.hasClass( classes.disabled ) && opts.type !== 'readonly' ) {
			that.val('');
		}
	} );

	this.dom.multiReturn.on( 'click', function () {
		that.multiRestore();
	} );

	// Field type extension methods - add a method to the field for the public
	// methods that each field type defines beyond the default ones that already
	// exist as part of this instance
	$.each( this.s.type, function ( name, fn ) {
		if ( typeof fn === 'function' && that[name] === undefined ) {
			that[ name ] = function () {
				var args = Array.prototype.slice.call( arguments );

				args.unshift( name );
				var ret = that._typeFn.apply( that, args );

				// Return the given value if there is one, or the field instance
				// for chaining if there is no value
				return ret === undefined ?
					that :
					ret;
			};
		}
	} );
};


Editor.Field.prototype = {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public
	 */
	def: function ( set ) {
		var opts = this.s.opts;

		if ( set === undefined ) {
			// Backwards compat
			var def = opts['default'] !== undefined ?
				opts['default'] :
				opts.def;

			return $.isFunction( def ) ?
				def() :
				def;
		}

		opts.def = set;
		return this;
	},

	disable: function () {
		this.dom.container.addClass( this.s.classes.disabled );

		this._typeFn( 'disable' );

		return this;
	},

	displayed: function () {
		var container = this.dom.container;

		return container.parents('body').length && container.css('display') != 'none' ?
			true :
			false;
	},

	enable: function () {
		this.dom.container.removeClass( this.s.classes.disabled );

		this._typeFn( 'enable' );

		return this;
	},

	enabled: function () {
		return this.dom.container.hasClass( this.s.classes.disabled ) === false;
	},

	error: function ( msg, fn ) {
		var classes = this.s.classes;

		// Add or remove the error class
		if ( msg ) {
			this.dom.container.addClass( classes.error );
		}
		else {
			this.dom.container.removeClass( classes.error );
		}

		this._typeFn( 'errorMessage', msg );

		return this._msg( this.dom.fieldError, msg, fn );
	},

	fieldInfo: function ( msg ) {
		return this._msg( this.dom.fieldInfo, msg );
	},

	isMultiValue: function () {
		return this.s.multiValue && this.s.multiIds.length !== 1;
	},

	inError: function () {
		return this.dom.container.hasClass( this.s.classes.error );
	},

	input: function () {
		return this.s.type.input ?
			this._typeFn( 'input' ) :
			$('input, select, textarea', this.dom.container);
	},

	focus: function () {
		if ( this.s.type.focus ) {
			this._typeFn( 'focus' );
		}
		else {
			$('input, select, textarea', this.dom.container).focus();
		}

		return this;
	},

	get: function () {
		// When multi-value a single get is undefined
		if ( this.isMultiValue() ) {
			return undefined;
		}

		var val = this._typeFn( 'get' );
		return val !== undefined ?
			val :
			this.def();
	},

	hide: function ( animate ) {
		var el = this.dom.container;

		if ( animate === undefined ) {
			animate = true;
		}

		if ( this.s.host.display() && animate ) {
			el.slideUp();
		}
		else {
			el.css( 'display', 'none' );
		}
		return this;
	},

	label: function ( str ) {
		var label = this.dom.label;
		var labelInfo = this.dom.labelInfo.detach();

		if ( str === undefined ) {
			return label.html();
		}

		label.html( str );
		label.append( labelInfo );
		return this;
	},

	labelInfo: function ( msg ) {
		return this._msg( this.dom.labelInfo, msg );
	},

	message: function ( msg, fn ) {
		return this._msg( this.dom.fieldMessage, msg, fn );
	},

	// There is no `multiVal()` as its arguments could be ambiguous
	// id is an idSrc value _only_
	multiGet: function ( id ) {
		var value;
		var multiValues = this.s.multiValues;
		var multiIds = this.s.multiIds;

		if ( id === undefined ) {
			// Get an object with the values for each item being edited
			value = {};

			for ( var i=0 ; i<multiIds.length ; i++ ) {
				value[ multiIds[i] ] = this.isMultiValue() ?
					multiValues[ multiIds[i] ] :
					this.val();
			}
		}
		else if ( this.isMultiValue() ) {
			// Individual value
			value = multiValues[ id ];
		}
		else {
			// Common value
			value = this.val();
		}

		return value;
	},
	
	multiRestore: function () {
		this.s.multiValue = true;
		this._multiValueCheck();
	},

	multiSet: function ( id, val )
	{
		var multiValues = this.s.multiValues;
		var multiIds = this.s.multiIds;

		if ( val === undefined ) {
			val = id;
			id = undefined;
		}

		// Set
		var set = function ( idSrc, val ) {
			// Get an individual item's value - add the id to the edit ids if
			// it isn't already in the set.
			if ( $.inArray( multiIds ) === -1 ) {
				multiIds.push( idSrc );
			}

			multiValues[ idSrc ] = val;
		};

		if ( $.isPlainObject( val ) && id === undefined ) {
			// idSrc / value pairs passed in
			$.each( val, function ( idSrc, innerVal ) {
				set( idSrc, innerVal );
			} );
		}
		else if ( id === undefined ) {
			// Set same value for all existing ids
			$.each( multiIds, function ( i, idSrc ) {
				set( idSrc, val );
			} );
		}
		else {
			// Setting an individual property
			set( id, val );
		}

		this.s.multiValue = true;
		this._multiValueCheck();

		return this;
	},

	name: function () {
		return this.s.opts.name;
	},

	node: function () {
		return this.dom.container[0];
	},

	// multiCheck is not publically documented
	set: function ( val, multiCheck ) {
		var decodeFn = function ( d ) {
			return typeof d !== 'string' ?
				d :
				d
					.replace(/&gt;/g, '>')
					.replace(/&lt;/g, '<')
					.replace(/&amp;/g, '&')
					.replace(/&quot;/g, '"')
					.replace(/&#39;/g, '\'')
					.replace(/&#10;/g, '\n');
		};

		this.s.multiValue = false;

		var decode = this.s.opts.entityDecode;
		if ( decode === undefined || decode === true ) {
			if ( $.isArray( val ) ) {
				for ( var i=0, ien=val.length ; i<ien ; i++ ) {
					val[i] = decodeFn( val[i] );
				}
			}
			else {
				val = decodeFn( val );
			}
		}

		this._typeFn( 'set', val );

		if ( multiCheck === undefined || multiCheck === true ) {
			this._multiValueCheck();
		}

		return this;
	},

	show: function ( animate ) {
		var el = this.dom.container;

		if ( animate === undefined ) {
			animate = true;
		}

		if ( this.s.host.display() && animate ) {
			el.slideDown();
		}
		else {
			el.css( 'display', 'block' );
		}
		return this;
	},

	val: function ( val ) {
		return val === undefined ?
			this.get() :
			this.set( val );
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Internal - Called from Editor only and are not publicly documented -
	 * these APIs can change!
	 */
	compare: function ( value, original ) {
		var compare = this.s.opts.compare || _deepCompare;
		return compare( value, original );
	},

	dataSrc: function () {
		return this.s.opts.data;
	},

	destroy: function () {
		// remove element
		this.dom.container.remove();

		// field's own destroy method if there is one
		this._typeFn( 'destroy' );
		return this;
	},

	multiEditable: function () {
		return this.s.opts.multiEditable;
	},

	multiIds: function () {
		return this.s.multiIds;
	},

	multiInfoShown: function ( show ) {
		this.dom.multiInfo.css( { display: show ? 'block' : 'none' } );
	},

	multiReset: function () {
		this.s.multiIds = [];
		this.s.multiValues = {};
	},

	valFromData: null,

	valToData: null,


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private
	 */
	_errorNode: function () {
		return this.dom.fieldError;
	},

	_msg: function ( el, msg, fn ) {
		if ( msg === undefined ) {
			return el.html();
		}

		if ( typeof msg === 'function' ) {
			var editor = this.s.host;
			msg = msg( editor, new DataTable.Api( editor.s.table ) );
		}

		if ( el.parent().is(":visible") ) {
			el.html( msg );

			if ( msg ) {
				el.slideDown( fn ); // fn can be undefined - so jQuery won't execute it
			}
			else {
				el.slideUp( fn );
			}
		}
		else {
			// Not visible, so immediately set, or blank out the element
			el
				.html( msg || '' )
				.css( 'display', msg ? 'block' : 'none' );

			if ( fn ) {
				fn();
			}
		}

		return this;
	},

	_multiValueCheck: function () {
		var last;
		var ids = this.s.multiIds;
		var values = this.s.multiValues;
		var isMultiValue = this.s.multiValue;
		var isMultiEditable = this.s.opts.multiEditable;
		var val;
		var different = false;

		if ( ids ) {
			for ( var i=0 ; i<ids.length ; i++ ) {
				val = values[ ids[i] ];

				if ( i > 0 && ! _deepCompare( val, last ) ) {
					different = true;
					break;
				}

				last = val;
			}
		}

		if ( (different && isMultiValue) || (!isMultiEditable && this.isMultiValue()) ) {
			// Different values or same values, but not multiple editable
			this.dom.inputControl.css( { display: 'none' } );
			this.dom.multi.css( { display: 'block' } );
		}
		else {
			// All the same value
			this.dom.inputControl.css( { display: 'block' } );
			this.dom.multi.css( { display: 'none' } );

			if ( isMultiValue && ! different ) {
				this.set( last, false );
			}
		}

		this.dom.multiReturn.css( {
			display: ids && ids.length > 1 && different && ! isMultiValue ?
				'block' :
				'none'
		} );

		// Update information label
		var i18n =  this.s.host.i18n.multi;
		this.dom.multiInfo.html( isMultiEditable ? i18n.info : i18n.noMulti);
		this.dom.multi.toggleClass( this.s.classes.multiNoEdit, ! isMultiEditable );

		this.s.host._multiInfo();

		return true;
	},

	_typeFn: function ( name /*, ... */ ) {
		// Remove the name from the arguments list, so the rest can be passed
		// straight into the field type
		var args = Array.prototype.slice.call( arguments );
		args.shift();

		// Insert the options as the first parameter - all field type methods
		// take the field's configuration object as the first parameter
		args.unshift( this.s.opts );

		var fn = this.s.type[ name ];
		if ( fn ) {
			return fn.apply( this.s.host, args );
		}
	}
};


Editor.Field.models = {};


/**
 * Initialisation options that can be given to Editor.Field at initialisation
 * time.
 *  @namespace
 */
Editor.Field.defaults = {
	/**
	 * Class name to assign to the field's container element (in addition to the other
	 * classes that Editor assigns by default).
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"className": "",

	/**
	 * The data property (`mData` in DataTables terminology) that is used to
	 * read from and write to the table. If not given then it will take the same
	 * value as the `name` that is given in the field object. Note that `data`
	 * can be given as null, which will result in Editor not using a DataTables
	 * row property for the value of the field for either getting or setting
	 * data.
	 *
	 * In previous versions of Editor (1.2-) this was called `dataProp`. The old
	 * name can still be used for backwards compatibility, but the new form is
	 * preferred.
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"data": "",

	/**
	 * The default value for the field. Used when creating new rows (editing will
	 * use the currently set value). If given as a function the function will be
	 * executed and the returned value used as the default
	 *
	 * In Editor 1.2 and earlier this field was called `default` - however
	 * `default` is a reserved word in Javascript, so it couldn't be used
	 * unquoted. `default` will still work with Editor 1.3, but the new property
	 * name of `def` is preferred.
	 *  @type string|function
	 *  @default <i>Empty string</i>
	 */
	"def": "",

	/**
	 * Helpful information text about the field that is shown below the input control.
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"fieldInfo": "",

	/**
	 * The ID of the field. This is used by the `label` HTML tag as the "for" attribute 
	 * improved accessibility. Although this using this parameter is not mandatory,
	 * it is a good idea to assign the ID to the DOM element that is the input for the
	 * field (if this is applicable).
	 *  @type string
	 *  @default <i>Calculated</i>
	 */
	"id": "",

	/**
	 * The label to display for the field input (i.e. the name that is visually 
	 * assigned to the field).
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"label": "",

	/**
	 * Helpful information text about the field that is shown below the field label.
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"labelInfo": "",

	/**
	 * The name for the field that is submitted to the server. This is the only
	 * mandatory parameter in the field description object.
	 *  @type string
	 *  @default <i>null</i>
	 */
	"name": null,

	/**
	 * The input control that is presented to the end user. The options available 
	 * are defined by {@link Editor.fieldTypes} and any extensions made 
	 * to that object.
	 *  @type string
	 *  @default text
	 */
	"type": "text",

	/**
	 * Information message for the field - expected to be dynamic
	 *  @type string
	 *  @default <i>Empty string</i>
	 */
	"message": "",

	/**
	 * Allow a field to be editable when multiple rows are selected
	 *  @type boolean
	 *  @default  true
	 */
	"multiEditable": true
};



/**
 * 
 *  @namespace
 */
Editor.Field.models.settings = {
	type: null,
	name: null,
	classes: null,
	opts: null,
	host: null
};



/**
 * 
 *  @namespace
 */
Editor.Field.models.dom = {
	container: null,
	label: null,
	labelInfo: null,
	fieldInfo: null,
	fieldError: null,
	fieldMessage: null
};


/*
 * Models
 */

/**
 * Object models container, for the various models that DataTables has available
 * to it. These models define the objects that are used to hold the active state
 * and configuration of the table.
 *  @namespace
 */
Editor.models = {};


/**
 * Editor makes very few assumptions about how its form will actually be
 * displayed to the end user (where in the DOM, interaction etc), instead
 * focusing on providing form interaction controls only. To actually display
 * a form in the browser we need to use a display controller, and then select
 * which one we want to use at initialisation time using the `display`
 * option. For example a display controller could display the form in a
 * lightbox (as the default display controller does), it could completely
 * empty the document and put only the form in place, ir could work with
 * DataTables to use `fnOpen` / `fnClose` to show the form in a "details" row
 * and so on.
 *
 * Editor has two built-in display controllers ('lightbox' and 'envelope'),
 * but others can readily be created and installed for use as plug-ins. When
 * creating a display controller plug-in you **must** implement the methods
 * in this control. Additionally when closing the display internally you
 * **must** trigger a `requestClose` event which Editor will listen
 * for and act upon (this allows Editor to ask the user if they are sure
 * they want to close the form, for example).
 *  @namespace
 */
Editor.models.displayController = {
	/**
	 * Initialisation method, called by Editor when itself, initialises.
	 *  @param {object} dte The DataTables Editor instance that has requested
	 *    the action - this allows access to the Editor API if required.
	 *  @returns {object} The object that Editor will use to run the 'open'
	 *    and 'close' methods against. If static methods are used then
	 *    just return the object that holds the init, open and close methods,
	 *    however, this allows the display to be created with a 'new'
	 *    instance of an object is the display controller calls for that.
	 *  @type function
	 */
	"init": function ( dte ) {},

	/**
	 * Display the form (add it to the visual display in the document)
	 *  @param {object} dte The DataTables Editor instance that has requested
	 *    the action - this allows access to the Editor API if required.
	 *  @param {element} append The DOM node that contains the form to be
	 *    displayed
	 *  @param {function} [fn] Callback function that is to be executed when
	 *    the form has been displayed. Note that this parameter is optional.
	 */
	"open": function ( dte, append, fn ) {},

	/**
	 * Hide the form (remove it form the visual display in the document)
	 *  @param {object} dte The DataTables Editor instance that has requested
	 *    the action - this allows access to the Editor API if required.
	 *  @param {function} [fn] Callback function that is to be executed when
	 *    the form has been hidden. Note that this parameter is optional.
	 */
	"close": function ( dte, fn ) {}
};



/**
 * Model object for input types which are available to fields (assigned to
 * {@link Editor.fieldTypes}). Any plug-ins which add additional
 * input types to Editor **must** implement the methods in this object 
 * (dummy functions are given in the model so they can be used as defaults
 * if extending this object).
 *
 * All functions in the model are executed in the Editor's instance scope,
 * so you have full access to the settings object and the API methods if
 * required.
 *  @namespace
 *  @example
 *    // Add a simple text input (the 'text' type that is built into Editor
 *    // does this, so you wouldn't implement this exactly as show, but it
 *    // it is a good example.
 *
 *    var Editor = $.fn.Editor;
 *
 *    Editor.fieldTypes.myInput = $.extend( true, {}, Editor.models.type, {
 *      "create": function ( conf ) {
 *        // We store the 'input' element in the configuration object so
 *        // we can easily access it again in future.
 *        conf._input = document.createElement('input');
 *        conf._input.id = conf.id;
 *        return conf._input;
 *      },
 *    
 *      "get": function ( conf ) {
 *        return conf._input.value;
 *      },
 *    
 *      "set": function ( conf, val ) {
 *        conf._input.value = val;
 *      },
 *    
 *      "enable": function ( conf ) {
 *        conf._input.disabled = false;
 *      },
 *    
 *      "disable": function ( conf ) {
 *        conf._input.disabled = true;
 *      }
 *    } );
 */
Editor.models.fieldType = {
	/**
	 * Create the field - this is called when the field is added to the form.
	 * Note that this is called at initialisation time, or when the 
	 * {@link Editor#add} API method is called, not when the form is displayed. 
	 * If you need to know when the form is shown, you can use the API to listen 
	 * for the `open` event.
	 *  @param {object} conf The configuration object for the field in question:
	 *    {@link Editor.models.field}.
	 *  @returns {element|null} The input element (or a wrapping element if a more
	 *    complex input is required) or null if nothing is to be added to the
	 *    DOM for this input type.
	 *  @type function
	 */
	"create": function ( conf ) {},

	/**
	 * Get the value from the field
	 *  @param {object} conf The configuration object for the field in question:
	 *    {@link Editor.models.field}.
	 *  @returns {*} The value from the field - the exact value will depend on the
	 *    formatting required by the input type control.
	 *  @type function
	 */
	"get": function ( conf ) {},

	/**
	 * Set the value for a field
	 *  @param {object} conf The configuration object for the field in question:
	 *    {@link Editor.models.field}.
	 *  @param {*} val The value to set the field to - the exact value will
	 *    depend on the formatting required by the input type control.
	 *  @type function
	 */
	"set": function ( conf, val ) {},

	/**
	 * Enable the field - i.e. allow user interface
	 *  @param {object} conf The configuration object for the field in question:
	 *    {@link Editor.models.field}.
	 *  @type function
	 */
	"enable": function ( conf ) {},

	/**
	 * Disable the field - i.e. disallow user interface
	 *  @param {object} conf The configuration object for the field in question:
	 *    {@link Editor.models.field}.
	 *  @type function
	 */
	"disable": function ( conf ) {}
};



/**
 * Settings object for Editor - this provides the state for each instance of
 * Editor and can be accessed through the instance's `s` property. Note that the
 * settings object is considered to be "private" and thus is liable to change
 * between versions. As such if you do read any of the setting parameters,
 * please keep this in mind when upgrading!
 *  @namespace
 */
Editor.models.settings = {
	/**
	 * URL to submit Ajax data to.
	 * This is directly set by the initialisation parameter / default of the same name.
	 *  @type string
	 *  @default null
	 */
	"ajaxUrl": null,

	/**
	 * Ajax submit function.
	 * This is directly set by the initialisation parameter / default of the same name.
	 *  @type function
	 *  @default null
	 */
	"ajax": null,

	/**
	 * Data source for get and set data actions. This allows Editor to perform
	 * as an Editor for virtually any data source simply by defining additional
	 * data sources.
	 *  @type object
	 *  @default null
	 */
	"dataSource": null,

	/**
	 * DataTable selector, can be anything that the Api supports
	 * This is directly set by the initialisation parameter / default of the same name.
	 *  @type string
	 *  @default null
	 */
	"domTable": null,

	/**
	 * The initialisation object that was given by the user - stored for future reference.
	 * This is directly set by the initialisation parameter / default of the same name.
	 *  @type string
	 *  @default null
	 */
	"opts": null,

	/**
	 * The display controller object for the Form.
	 * This is directly set by the initialisation parameter / default of the same name.
	 *  @type string
	 *  @default null
	 */
	"displayController": null,

	/**
	 * The form fields - see {@link Editor.models.field} for details of the 
	 * objects held in this array.
	 *  @type object
	 *  @default null
	 */
	"fields": {},

	/**
	 * Field order - order that the fields will appear in on the form. Array of strings,
	 * the names of the fields.
	 *  @type array
	 *  @default null
	 */
	"order": [],

	/**
	 * The ID of the row being edited (set to -1 on create and remove actions)
	 *  @type string
	 *  @default null
	 */
	"id": -1,

	/**
	 * Flag to indicate if the form is currently displayed or not and what type of display
	 *  @type string
	 *  @default null
	 */
	"displayed": false,

	/**
	 * Flag to indicate if the form is current in a processing state (true) or not (false)
	 *  @type string
	 *  @default null
	 */
	"processing": false,

	/**
	 * Developer provided identifier for the elements to be edited (i.e. at
	 * `dt-type row-selector` to select rows to edit or delete.
	 *  @type array
	 *  @default null
	 */
	"modifier": null,

	/**
	 * The current form action - 'create', 'edit' or 'remove'. If no current action then
	 * it is set to null.
	 *  @type string
	 *  @default null
	 */
	"action": null,

	/**
	 * JSON property from which to read / write the row's ID property.
	 *  @type string
	 *  @default null
	 */
	"idSrc": null,

	/**
	 * Unique instance counter to be able to remove events
	 */
	"unique": 0
};



/**
 * Model of the buttons that can be used with the {@link Editor#buttons}
 * method for creating and displaying buttons (also the {@link Editor#button}
 * argument option for the {@link Editor#create}, {@link Editor#edit} and 
 * {@link Editor#remove} methods). Although you don't need to extend this object,
 * it is available for reference to show the options available.
 *  @namespace
 */
Editor.models.button = {
	/**
	 * The text to put into the button. This can be any HTML string you wish as 
	 * it will be rendered as HTML (allowing images etc to be shown inside the
	 * button).
	 *  @type string
	 *  @default null
	 */
	"label": null,

	/**
	 * Callback function which the button is activated. For example for a 'submit' 
	 * button you would call the {@link Editor#submit} API method, while for a cancel button
	 * you would call the {@link Editor#close} API method. Note that the function is executed 
	 * in the scope of the Editor instance, so you can call the Editor's API methods 
	 * using the `this` keyword.
	 *  @type function
	 *  @default null
	 */
	"fn": null,

	/**
	 * The CSS class(es) to apply to the button which can be useful for styling buttons 
	 * which preform different functions each with a distinctive visual appearance.
	 *  @type string
	 *  @default null
	 */
	"className": null
};



/**
 * This is really an internal namespace
 *
 *  @namespace
 */
Editor.models.formOptions = {
	/**
	 * Action to take when the return key is pressed when focused in a form
	 * element. Cam be `submit` or `none`. Could also be `blur` or `close`, but
	 * why would you ever want that. Replaces `submitOnReturn` from 1.4.
	 * 
	 * @type string
	 */
	onReturn: 'submit',

	/**
	 * Action to take on blur. Can be `close`, `submit` or `none`. Replaces
	 * `submitOnBlur` from 1.4
	 * 
	 * @type string
	 */
	onBlur: 'close',

	/**
	 * Action to take when the lightbox background is clicked - can be `close`,
	 * `submit`, `blur` or `none`. Replaces `blurOnBackground` from 1.4
	 * 
	 * @type string
	 */
	onBackground: 'blur',

	/**
	 * Close for at the end of the Ajax request. Can be `close` or `none`.
	 * Replaces `closeOnComplete` from 1.4.
	 * 
	 * @type string
	 */
	onComplete: 'close',

	/**
	 * Action to take when the `esc` key is pressed when focused in the form -
	 * can be `close`, `submit`, `blur` or `none`
	 * 
	 * @type string
	 */
	onEsc: 'close',

	/**
	 * Action to take when a field error is detected in the returned JSON - can
	 * be `focus` or `none`
	 * 
	 * @type string
	 */
	onFieldError: 'focus',

	/**
	 * Data to submit to the server when submitting a form. If an option is
	 * selected that results in no data being submitted, the Ajax request will
	 * not be made Can be `all`, `changed` or `allIfChanged`. This effects the
	 * edit action only.
	 *
	 * @type string
	 */
	submit: 'all',

	/**
	 * Field identifier to focus on
	 * 
	 * @type null|integer|string
	 */
	focus: 0,

	/**
	 * Buttons to show in the form
	 * 
	 * @type string|boolean|array|object
	 */
	buttons: true,

	/**
	 * Form title
	 * 
	 * @type string|boolean
	 */
	title: true,

	/**
	 * Form message
	 * 
	 * @type string|boolean
	 */
	message: true,

	/**
	 * DataTables redraw option
	 * 
	 * @type string|boolean
	 */
	drawType: false
};


/*
 * Display controllers
 */

/**
 * Display controllers. See {@link Editor.models.displayController} for
 * full information about the display controller options for Editor. The display
 * controllers given in this object can be utilised by specifying the
 * {@link Editor.defaults.display} option.
 *  @namespace
 */
Editor.display = {};


(function(window, document, $, DataTable) {


var self;

Editor.display.lightbox = $.extend( true, {}, Editor.models.displayController, {
	/*
	 * API methods
	 */
	"init": function ( dte ) {
		self._init();
		return self;
	},

	"open": function ( dte, append, callback ) {
		if ( self._shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		self._dte = dte;

		var content = self._dom.content;
		content.children().detach();
		content
			.append( append )
			.append( self._dom.close );

		self._shown = true;
		self._show( callback );
	},

	"close": function ( dte, callback ) {
		if ( !self._shown ) {
			if ( callback ) {
				callback();
			}
			return;
		}

		self._dte = dte;
		self._hide( callback );

		self._shown = false;
	},

	node: function ( dte ) {
		return self._dom.wrapper[0];
	},


	/*
	 * Private methods
	 */
	"_init": function () {
		if ( self._ready ) {
			return;
		}

		var dom = self._dom;
		dom.content = $('div.DTED_Lightbox_Content', self._dom.wrapper);

		dom.wrapper.css( 'opacity', 0 );
		dom.background.css( 'opacity', 0 );
	},


	"_show": function ( callback ) {
		var that = this;
		var dom = self._dom;

		// Mobiles have very poor position fixed abilities, so we need to know
		// when using mobile A media query isn't good enough
		if ( window.orientation !== undefined ) {
			$('body').addClass( 'DTED_Lightbox_Mobile' );
		}

		// Adjust size for the content
		dom.content.css( 'height', 'auto' );
		dom.wrapper.css( {
			top: -self.conf.offsetAni
		} );

		$('body')
			.append( self._dom.background )
			.append( self._dom.wrapper );

		self._heightCalc();

		dom.wrapper
			.stop()
				.animate( {
				opacity: 1,
				top: 0
			}, callback );

		dom.background
			.stop()
			.animate( {
				opacity: 1
			} );

		// Terrible Chrome workaround. Since m53 the footer would be incorrectly
		// offset. This triggers a rerender. See thread 38145
		setTimeout( function () {
			$('div.DTE_Footer').css( 'text-indent', -1 );
		}, 10 );

		// Event handlers - assign on show (and unbind on hide) rather than init
		// since we might need to refer to different editor instances - 12563
		dom.close.bind( 'click.DTED_Lightbox', function (e) {
			self._dte.close();
		} );

		dom.background.bind( 'click.DTED_Lightbox', function (e) {
			self._dte.background();
		} );

		$('div.DTED_Lightbox_Content_Wrapper', dom.wrapper).bind( 'click.DTED_Lightbox', function (e) {
			if ( $(e.target).hasClass('DTED_Lightbox_Content_Wrapper') ) {
				self._dte.background();
			}
		} );

		$(window).bind( 'resize.DTED_Lightbox', function () {
			self._heightCalc();
		} );

		self._scrollTop = $('body').scrollTop();

		// For smaller screens we need to hide the other elements in the
		// document since iOS and Android both mess up display:fixed when
		// the virtual keyboard is shown
		if ( window.orientation !== undefined ) {
			var kids = $('body').children().not( dom.background ).not( dom.wrapper );
			$('body').append( '<div class="DTED_Lightbox_Shown"/>' );
			$('div.DTED_Lightbox_Shown').append( kids );
		}
	},


	"_heightCalc": function () {
		// Set the max-height for the form content
		var dom = self._dom;
		var maxHeight = $(window).height() - (self.conf.windowPadding*2) -
			$('div.DTE_Header', dom.wrapper).outerHeight() -
			$('div.DTE_Footer', dom.wrapper).outerHeight();

		$('div.DTE_Body_Content', dom.wrapper).css(
			'maxHeight',
			maxHeight
		);
	},


	"_hide": function ( callback ) {
		var dom = self._dom;

		if ( !callback ) {
			callback = function () {};
		}

		if ( window.orientation !== undefined  ) {
			var show = $('div.DTED_Lightbox_Shown');
			show.children().appendTo('body');
			show.remove();
		}

		// Restore scroll state
		$('body')
			.removeClass( 'DTED_Lightbox_Mobile' )
			.scrollTop( self._scrollTop );

		dom.wrapper
			.stop()
			.animate( {
				opacity: 0,
				top: self.conf.offsetAni
			}, function () {
				$(this).detach();
				callback();
			} );

		dom.background
			.stop()
			.animate( {
				opacity: 0
			}, function () {
				$(this).detach();
			} );

		// Event handlers
		dom.close.unbind( 'click.DTED_Lightbox' );
		dom.background.unbind( 'click.DTED_Lightbox' );
		$('div.DTED_Lightbox_Content_Wrapper', dom.wrapper).unbind( 'click.DTED_Lightbox' );
		$(window).unbind( 'resize.DTED_Lightbox' );
	},


	/*
	 * Private properties
	 */
	"_dte": null,
	"_ready": false,
	"_shown": false,
	"_dom": {
		"wrapper": $(
			'<div class="DTED DTED_Lightbox_Wrapper">'+
				'<div class="DTED_Lightbox_Container">'+
					'<div class="DTED_Lightbox_Content_Wrapper">'+
						'<div class="DTED_Lightbox_Content">'+
						'</div>'+
					'</div>'+
				'</div>'+
			'</div>'
		),

		"background": $(
			'<div class="DTED_Lightbox_Background"><div/></div>'
		),

		"close": $(
			'<div class="DTED_Lightbox_Close"></div>'
		),

		"content": null
	}
} );

self = Editor.display.lightbox;

self.conf = {
	"offsetAni": 25,
	"windowPadding": 25
};


}(window, document, jQuery, jQuery.fn.dataTable));



(function(window, document, $, DataTable) {


var self;

Editor.display.envelope = $.extend( true, {}, Editor.models.displayController, {
	/*
	 * API methods
	 */
	"init": function ( dte ) {
		self._dte = dte;
		self._init();
		return self;
	},


	"open": function ( dte, append, callback ) {
		self._dte = dte;
		$(self._dom.content).children().detach();
		self._dom.content.appendChild( append );
		self._dom.content.appendChild( self._dom.close );

		self._show( callback );
	},


	"close": function ( dte, callback ) {
		self._dte = dte;
		self._hide( callback );
	},

	node: function ( dte ) {
		return self._dom.wrapper[0];
	},


	/*
	 * Private methods
	 */
	"_init": function () {
		if ( self._ready ) {
			return;
		}

		self._dom.content = $('div.DTED_Envelope_Container', self._dom.wrapper)[0];

		document.body.appendChild( self._dom.background );
		document.body.appendChild( self._dom.wrapper );

		// For IE6-8 we need to make it a block element to read the opacity...
		self._dom.background.style.visbility = 'hidden';
		self._dom.background.style.display = 'block';
		self._cssBackgroundOpacity = $(self._dom.background).css('opacity');
		self._dom.background.style.display = 'none';
		self._dom.background.style.visbility = 'visible';
	},


	"_show": function ( callback ) {
		var that = this;
		var formHeight;

		if ( !callback ) {
			callback = function () {};
		}

		// Adjust size for the content
		self._dom.content.style.height = 'auto';

		var style = self._dom.wrapper.style;
		style.opacity = 0;
		style.display = 'block';

		var targetRow = self._findAttachRow();
		var height = self._heightCalc();
		var width = targetRow.offsetWidth;

		style.display = 'none';
		style.opacity = 1;

		// Prep the display
		self._dom.wrapper.style.width = width+"px";
		self._dom.wrapper.style.marginLeft = -(width/2)+"px";
		self._dom.wrapper.style.top = ($(targetRow).offset().top + targetRow.offsetHeight)+"px";
		self._dom.content.style.top = ((-1 * height) - 20)+"px";

		// Start animating in the background
		self._dom.background.style.opacity = 0;
		self._dom.background.style.display = 'block';
		$(self._dom.background).animate( {
			'opacity': self._cssBackgroundOpacity
		}, 'normal' );

		// Animate in the display
		$(self._dom.wrapper).fadeIn();

		// Slide the slider down to 'open' the view
		if ( self.conf.windowScroll ) {
			// Scroll the window so we can see the editor first
			$('html,body').animate( {
				"scrollTop": $(targetRow).offset().top + targetRow.offsetHeight - self.conf.windowPadding
			}, function () {
				// Now open the editor
				$(self._dom.content).animate( {
					"top": 0
				}, 600, callback );
			} );
		}
		else {
			// Just open the editor without moving the document position
			$(self._dom.content).animate( {
				"top": 0
			}, 600, callback );
		}

		// Event handlers
		$(self._dom.close).bind( 'click.DTED_Envelope', function (e) {
			self._dte.close();
		} );

		$(self._dom.background).bind( 'click.DTED_Envelope', function (e) {
			self._dte.background();
		} );

		$('div.DTED_Lightbox_Content_Wrapper', self._dom.wrapper).bind( 'click.DTED_Envelope', function (e) {
			if ( $(e.target).hasClass('DTED_Envelope_Content_Wrapper') ) {
				self._dte.background();
			}
		} );

		$(window).bind( 'resize.DTED_Envelope', function () {
			self._heightCalc();
		} );
	},


	"_heightCalc": function () {
		var formHeight;

		formHeight = self.conf.heightCalc ?
			self.conf.heightCalc( self._dom.wrapper ) :
			$(self._dom.content).children().height();

		// Set the max-height for the form content
		var maxHeight = $(window).height() - (self.conf.windowPadding*2) -
			$('div.DTE_Header', self._dom.wrapper).outerHeight() -
			$('div.DTE_Footer', self._dom.wrapper).outerHeight();

		$('div.DTE_Body_Content', self._dom.wrapper).css('maxHeight', maxHeight);

		return $(self._dte.dom.wrapper).outerHeight();
	},


	"_hide": function ( callback ) {
		if ( !callback ) {
			callback = function () {};
		}

		$(self._dom.content).animate( {
			"top": -(self._dom.content.offsetHeight+50)
		}, 600, function () {
			$([self._dom.wrapper, self._dom.background]).fadeOut( 'normal', callback );
		} );

		// Event handlers
		$(self._dom.close).unbind( 'click.DTED_Lightbox' );
		$(self._dom.background).unbind( 'click.DTED_Lightbox' );
		$('div.DTED_Lightbox_Content_Wrapper', self._dom.wrapper).unbind( 'click.DTED_Lightbox' );
		$(window).unbind( 'resize.DTED_Lightbox' );
	},


	"_findAttachRow": function () {
		var dt = $(self._dte.s.table).DataTable();

		// Figure out where we want to put the form display
		if ( self.conf.attach === 'head' ) {
			return dt.table().header();
		}
		else if ( self._dte.s.action === 'create' ) {
			return dt.table().header();
		}
		else {
			return dt.row( self._dte.s.modifier ).node();
		}
	},


	/*
	 * Private properties
	 */
	"_dte": null,
	"_ready": false,
	"_cssBackgroundOpacity": 1, // read from the CSS dynamically, but stored for future reference


	"_dom": {
		"wrapper": $(
			'<div class="DTED DTED_Envelope_Wrapper">'+
				'<div class="DTED_Envelope_Shadow"></div>'+
				'<div class="DTED_Envelope_Container"></div>'+
			'</div>'
		)[0],

		"background": $(
			'<div class="DTED_Envelope_Background"><div/></div>'
		)[0],

		"close": $(
			'<div class="DTED_Envelope_Close">&times;</div>'
		)[0],

		"content": null
	}
} );


// Assign to 'self' for easy referencing of our own object!
self = Editor.display.envelope;


// Configuration object - can be accessed globally using 
// $.fn.Editor.display.envelope.conf (!)
self.conf = {
	"windowPadding": 50,
	"heightCalc": null,
	"attach": "row",
	"windowScroll": true
};


}(window, document, jQuery, jQuery.fn.dataTable));


/*
 * Prototype includes
 */


/**
 * Add a new field to the from. This is the method that is called automatically when
 * fields are given in the initialisation objects as {@link Editor.defaults.fields}.
 *  @memberOf Editor
 *  @param {object|array} field The object that describes the field (the full
 *    object is described by {@link Editor.model.field}. Note that multiple
 *    fields can be given by passing in an array of field definitions.
 *  @param {string} [after] Existing field to insert the new field after. This
 *    can be `undefined` (insert at end), `null` (insert at start) or `string`
 *    the field name to insert after.
 */
Editor.prototype.add = function ( cfg, after )
{
	// Allow multiple fields to be added at the same time
	if ( $.isArray( cfg ) ) {
		for ( var i=0, iLen=cfg.length ; i<iLen ; i++ ) {
			this.add( cfg[i] );
		}
	}
	else {
		var name = cfg.name;

		if ( name === undefined ) {
			throw "Error adding field. The field requires a `name` option";
		}

		if ( this.s.fields[ name ] ) {
			throw "Error adding field '"+name+"'. A field already exists with this name";
		}

		// Allow the data source to add / modify the field properties
		// Dev: would this be better as an event `preAddField`? And have the
		// data sources init only once, but can listen for such events? More
		// complexity, but probably more flexible...
		this._dataSource( 'initField', cfg );

		this.s.fields[ name ] = new Editor.Field( cfg, this.classes.field, this );

		if ( after === undefined ) {
			this.s.order.push( name );
		}
		else if ( after === null ) {
			this.s.order.unshift( name );
		}
		else {
			var idx = $.inArray( after, this.s.order );
			this.s.order.splice( idx+1, 0, name );
		}
	}

	this._displayReorder( this.order() );

	return this;
};


/**
 * Perform background activation tasks.
 * 
 * This is NOT publicly documented on the Editor web-site, but rather can be
 * used by display controller plug-ins to perform the required task on
 * background activation.
 *
 * @return {Editor} Editor instance, for chaining
 */
Editor.prototype.background = function ()
{
	var onBackground = this.s.editOpts.onBackground;

	if ( typeof onBackground === 'function' ) {
		onBackground( this );
	}
	else if ( onBackground === 'blur' ) {
		this.blur();
	}
	else if ( onBackground === 'close' ) {
		this.close();
	}
	else if ( onBackground === 'submit' ) {
		this.submit();
	}

	return this;
};


/**
 * Blur the currently displayed editor.
 *
 * A blur is different from a `close()` in that it might cause either a close or
 * the form to be submitted. A typical example of a blur would be clicking on
 * the background of the bubble or main editing forms - i.e. it might be a
 * close, or it might submit depending upon the configuration, while a click on
 * the close box is a very definite close.
 *
 * @return {Editor} Editor instance, for chaining
 */
Editor.prototype.blur = function ()
{
	this._blur();

	return this;
};



Editor.prototype.bubble = function ( cells, fieldNames, show, opts )
{
	var that = this;

	// Some other field in inline edit mode?
	if ( this._tidy( function () { that.bubble( cells, fieldNames, opts ); } ) ) {
		return this;
	}

	// Argument shifting
	if ( $.isPlainObject( fieldNames ) ) {
		opts = fieldNames;
		fieldNames = undefined;
		show = true;
	}
	else if ( typeof fieldNames === 'boolean' ) {
		show = fieldNames;
		fieldNames = undefined;
		opts = undefined;
	}

	if ( $.isPlainObject( show ) ) {
		opts = show;
		show = true;
	}

	if ( show === undefined ) {
		show = true;
	}

	opts = $.extend( {}, this.s.formOptions.bubble, opts );

	var editFields = this._dataSource( 'individual', cells, fieldNames );

	this._edit( cells, editFields, 'bubble' );

	var namespace = this._formOptions( opts );
	var ret = this._preopen( 'bubble' );
	if ( ! ret ) {
		return this;
	}

	// Keep the bubble in position on resize
	$(window).on( 'resize.'+namespace, function () {
		that.bubblePosition();
	} );

	// Store the nodes that are being used so the bubble can be positioned
	var nodes = [];
	this.s.bubbleNodes = nodes.concat.apply( nodes, _pluck( editFields, 'attach' ) );

	// Create container display
	var classes = this.classes.bubble;
	var background = $( '<div class="'+classes.bg+'"><div/></div>' );
	var container = $(
			'<div class="'+classes.wrapper+'">'+
				'<div class="'+classes.liner+'">'+
					'<div class="'+classes.table+'">'+
						'<div class="'+classes.close+'" />'+
						'<div class="DTE_Processing_Indicator"><span></div>'+
					'</div>'+
				'</div>'+
				'<div class="'+classes.pointer+'" />'+
			'</div>'
		);

	if ( show ) {
		container.appendTo( 'body' );
		background.appendTo( 'body' );
	}

	var liner = container.children().eq(0);
	var table = liner.children();
	var close = table.children();
	liner.append( this.dom.formError );
	table.prepend( this.dom.form );

	if ( opts.message ) {
		liner.prepend( this.dom.formInfo );
	}

	if ( opts.title ) {
		liner.prepend( this.dom.header );
	}

	if ( opts.buttons ) {
		table.append( this.dom.buttons );
	}

	var pair = $().add( container ).add( background );
	this._closeReg( function ( submitComplete ) {
		pair.animate(
			{ opacity: 0 },
			function () {
				pair.detach();

				$(window).off( 'resize.'+namespace );

				// Clear error messages "offline"
				that._clearDynamicInfo();
			}
		);
	} );

	// Close event handlers
	background.click( function () {
		that.blur();
	} );

	close.click( function () {
		that._close();
	} );

	this.bubblePosition();

	pair.animate( { opacity: 1 } );

	this._focus( this.s.includeFields, opts.focus );
	this._postopen( 'bubble' );

	return this;
};


/**
 * Reposition the editing bubble (`bubble()`) when it is visible. This can be
 * used to update the bubble position if other elements on the page change
 * position. Editor will automatically call this method on window resize.
 *
 * @return {Editor} Editor instance, for chaining
 */
Editor.prototype.bubblePosition = function ()
{
	var
		wrapper = $('div.DTE_Bubble'),
		liner = $('div.DTE_Bubble_Liner'),
		nodes = this.s.bubbleNodes;

	// Average the node positions to insert the container
	var position = { top: 0, left: 0, right: 0, bottom: 0 };

	$.each( nodes, function (i, node) {
		var pos = $(node).offset();
		node = $(node).get(0);

		position.top += pos.top;
		position.left += pos.left;
		position.right += pos.left + node.offsetWidth;
		position.bottom += pos.top + node.offsetHeight;
	} );

	position.top /= nodes.length;
	position.left /= nodes.length;
	position.right /= nodes.length;
	position.bottom /= nodes.length;

	var
		top = position.top,
		left = (position.left + position.right) / 2,
		width = liner.outerWidth(),
		visLeft = left - (width / 2),
		visRight = visLeft + width,
		docWidth = $(window).width(),
		padding = 15,
		classes = this.classes.bubble;

	wrapper.css( {
		top: top,
		left: left
	} );

	// Correct for overflow from the top of the document by positioning below
	// the field if needed
	if ( liner.length && liner.offset().top < 0 ) {
		wrapper
			.css( 'top', position.bottom )
			.addClass( 'below' );
	}
	else {
		wrapper.removeClass( 'below' );
	}

	// Attempt to correct for overflow to the right of the document
	if ( visRight+padding > docWidth ) {
		var diff = visRight - docWidth;

		// If left overflowing, that takes priority
		liner.css( 'left', visLeft < padding ?
			-(visLeft-padding) :
			-(diff+padding)
		);
	}
	else {
		// Correct overflow to the left
		liner.css( 'left', visLeft < padding ? -(visLeft-padding) : 0 );
	}

	return this;
};


/**
 * Setup the buttons that will be shown in the footer of the form - calling this
 * method will replace any buttons which are currently shown in the form.
 *  @param {array|object} buttons A single button definition to add to the form or
 *    an array of objects with the button definitions to add more than one button.
 *    The options for the button definitions are fully defined by the
 *    {@link Editor.models.button} object.
 *  @param {string} buttons.text The text to put into the button. This can be any
 *    HTML string you wish as it will be rendered as HTML (allowing images etc to 
 *    be shown inside the button).
 *  @param {function} [buttons.action] Callback function which the button is activated.
 *    For example for a 'submit' button you would call the {@link Editor#submit} method,
 *    while for a cancel button you would call the {@link Editor#close} method. Note that
 *    the function is executed in the scope of the Editor instance, so you can call
 *    the Editor's API methods using the `this` keyword.
 *  @param {string} [buttons.className] The CSS class(es) to apply to the button
 *    which can be useful for styling buttons which preform different functions
 *    each with a distinctive visual appearance.
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.buttons = function ( buttons )
{
	var that = this;

	if ( buttons === '_basic' ) {
		// Special string to create a basic button - undocumented
		buttons = [ {
			text: this.i18n[ this.s.action ].submit,
			action: function () { this.submit(); }
		} ];
	}
	else if ( ! $.isArray( buttons ) ) {
		// Allow a single button to be passed in as an object with an array
		buttons = [ buttons ];
	}

	$(this.dom.buttons).empty();

	$.each( buttons, function ( i, btn ) {
		if ( typeof btn === 'string' ) {
			btn = {
				text: btn,
				action: function () { this.submit(); }
			};
		}

		var text = btn.text || btn.label;
		var action = btn.action || btn.fn;

		$( '<button/>', {
				'class': that.classes.form.button+(btn.className ? ' '+btn.className : '')
			} )
			.html( typeof text === 'function' ?
				text( that ) :
				text || ''
			)
			.attr( 'tabindex', btn.tabIndex !== undefined ? btn.tabIndex : 0 )
			.on( 'keyup', function (e) {
				if ( e.keyCode === 13 && action ) {
					action.call( that );
				}
			} )
			.on( 'keypress', function (e) {
				// Stop the browser activating the click event - if we don't
				// have this and the Ajax return is fast, the keyup in
				// `_formOptions()` might trigger another submit
				if ( e.keyCode === 13 ) {
					e.preventDefault();
				}
			} )
			.on( 'click', function (e) {
				e.preventDefault();

				if ( action ) {
					action.call( that );
				}
			} )
			.appendTo( that.dom.buttons );
	} );

	return this;
};


/**
 * Remove fields from the form (fields are those that have been added using the
 * {@link Editor#add} method or the `fields` initialisation option). A single,
 * multiple or all fields can be removed at a time based on the passed parameter.
 * Fields are identified by the `name` property that was given to each field
 * when added to the form.
 *  @param {string|array} [fieldName] Field or fields to remove from the form. If
 *    not given then all fields are removed from the form. If given as a string
 *    then the single matching field will be removed. If given as an array of
 *    strings, then all matching fields will be removed.
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Clear the form of current fields and then add a new field 
 *    // before displaying a 'create' display
 *    editor.clear();
 *    editor.add( {
 *      "label": "User name",
 *      "name": "username"
 *    } );
 *    editor.create( "Create user" );
 *
 *  @example
 *    // Remove an individual field
 *    editor.clear( "username" );
 *
 *  @example
 *    // Remove multiple fields
 *    editor.clear( [ "first_name", "last_name" ] );
 */
Editor.prototype.clear = function ( fieldName )
{
	var that = this;
	var fields = this.s.fields;

	if ( typeof fieldName === 'string' ) {
		// Remove an individual form element
		that.field(fieldName).destroy();
		delete fields[ fieldName ];

		var orderIdx = $.inArray( fieldName, this.s.order );
		this.s.order.splice( orderIdx, 1 );

		var includeIdx = $.inArray( fieldName, this.s.includeFields );
		if ( includeIdx !== -1 ) {
			this.s.includeFields.splice( includeIdx, 1 );
		}
	}
	else {
		$.each( this._fieldNames( fieldName ), function (i, name) {
			that.clear( name );
		} );
	}

	return this;
};


/**
 * Close the form display.
 * 
 * Note that `close()` will close any of the three Editor form types (main,
 * bubble and inline).
 *
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.close = function ()
{
	this._close( false );

	return this;
};


/**
 * Create a new record - show the form that allows the user to enter information
 * for a new row and then subsequently submit that data.
 *  @param {boolean} [show=true] Show the form or not.
 * 
 *  @example
 *    // Show the create form with a submit button
 *    editor
 *      .title( 'Add new record' )
 *      .buttons( {
 *        "label": "Save",
 *        "fn": function () {
 *          this.submit();
 *        }
 *      } )
 *      .create();
 * 
 *  @example
 *    // Don't show the form and automatically submit it after programatically 
 *    // setting the values of fields (and using the field defaults)
 *    editor
 *      create()
 *      set( 'name',   'Test user' )
 *      set( 'access', 'Read only' )
 *      submit();
 */
Editor.prototype.create = function ( arg1, arg2, arg3, arg4 )
{
	var that = this;
	var fields = this.s.fields;
	var count = 1;

	// Some other field in inline edit mode?
	if ( this._tidy( function () { that.create( arg1, arg2, arg3, arg4 ); } ) ) {
		return this;
	}

	// Multi-row creation support (only supported by the 1.3+ style of calling
	// this method, so a max of three arguments
	if ( typeof arg1 === 'number' ) {
		count = arg1;
		arg1 = arg2;
		arg2 = arg3;
	}

	// Set up the edit fields for submission
	this.s.editFields = {};
	for ( var i=0 ; i<count ; i++ ) {
		this.s.editFields[ i ] = {
			fields: this.s.fields
		};
	}

	var argOpts = this._crudArgs( arg1, arg2, arg3, arg4 );

	this.s.mode = 'main';
	this.s.action = "create";
	this.s.modifier = null;
	this.dom.form.style.display = 'block';

	this._actionClass();

	// Allow all fields to be displayed for the create form
	this._displayReorder( this.fields() );

	// Set the default for the fields
	$.each( fields, function ( name, field ) {
		field.multiReset();
		field.set( field.def() );
	} );

	this._event( 'initCreate' );
	this._assembleMain();
	this._formOptions( argOpts.opts );

	argOpts.maybeOpen();

	return this;
};

/**
 * Create a dependent link between two or more fields. This method is used to
 * listen for a change in a field's value which will trigger updating of the
 * form. This update can consist of updating an options list, changing values
 * or making fields hidden / visible.
 *
 * @param {string} parent The name of the field to listen to changes from
 * @param {string|object|function} url Callback definition. This can be:
 *   * A string, which will be used as a URL to submit the request for update to
 *   * An object, which is used to extend an Ajax object for the request. The
 *     `url` parameter must be specified.
 *   * A function, which is used as a callback, allowing non-ajax updates.
 * @return {Editor} Editor instance, for chaining
 */
Editor.prototype.dependent = function ( parent, url, opts ) {
	if ( $.isArray( parent ) ) {
		for ( var i=0, ien=parent.length ; i<ien ; i++ ) {
			this.dependent( parent[i], url, opts );
		}

		return this;
	}

	var that = this;
	var field = this.field( parent );
	var ajaxOpts = {
		type: 'POST',
		dataType: 'json'
	};

	opts = $.extend( {
		event: 'change',
		data: null,
		preUpdate: null,
		postUpdate: null
	}, opts );

	var update = function ( json ) {
		if ( opts.preUpdate ) {
			opts.preUpdate( json );
		}

		// Field specific
		$.each( {
			labels:   'label',
			options:  'update',
			values:   'val',
			messages: 'message',
			errors:   'error'
		}, function ( jsonProp, fieldFn ) {
			if ( json[ jsonProp ] ) {
				$.each( json[ jsonProp ], function ( field, val ) {
					that.field( field )[ fieldFn ]( val );
				} );
			}
		} );

		// Form level
		$.each( [ 'hide', 'show', 'enable', 'disable' ], function ( i, key ) {
			if ( json[ key ] ) {
				that[ key ]( json[ key ] );
			}
		} );

		if ( opts.postUpdate ) {
			opts.postUpdate( json );
		}
	};

	// Use a delegate handler to account for field elements which are added and
	// removed after `depenedent` has been called
	$(field.node()).on( opts.event, function (e) {
		// Make sure that it was one of the field's elements that triggered the ev
		if ( $( field.node() ).find( e.target ).length === 0 ) {
			return;
		}

		var data = {};
		data.rows = that.s.editFields ?
			_pluck( that.s.editFields, 'data' ) :
			null;
		data.row = data.rows ?
			data.rows[0] :
			null;
		data.values = that.val();

		if ( opts.data ) {
			var ret = opts.data( data );

			if ( ret ) {
				opts.data = ret;
			}
		}

		if ( typeof url === 'function' ) {
			var o = url( field.val(), data, update );

			if ( o ) {
				update( o );
			}
		}
		else {
			if ( $.isPlainObject( url ) ) {
				$.extend( ajaxOpts, url );
			}
			else {
				ajaxOpts.url = url;
			}

			$.ajax( $.extend( ajaxOpts, {
				url: url,
				data: data,
				success: update
			} ) );
		}
	} );

	return this;
};


/**
 * Destroy the Editor instance, cleaning up fields, display and event handlers
 */
Editor.prototype.destroy = function ()
{
	if ( this.s.displayed ) {
		this.close();
	}

	this.clear();

	var controller = this.s.displayController;
	if ( controller.destroy ) {
		controller.destroy( this );
	}

	$(document).off( '.dte'+this.s.unique );

	this.dom = null;
	this.s = null;
};


/**
 * Disable one or more field inputs, disallowing subsequent user interaction with the 
 * fields until they are re-enabled.
 *  @param {string|array} name The field name (from the `name` parameter given when
 *   originally setting up the field) to disable, or an array of field names to disable
 *   multiple fields with a single call.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show a 'create' record form, but with a field disabled
 *    editor.disable( 'account_type' );
 *    editor.create( 'Add new user', {
 *      "label": "Save",
 *      "fn": function () { this.submit(); }
 *    } );
 * 
 *  @example
 *    // Disable multiple fields by using an array of field names
 *    editor.disable( ['account_type', 'access_level'] );
 */
Editor.prototype.disable = function ( name )
{
	var that = this;

	$.each( this._fieldNames( name ), function ( i, n ) {
		that.field(n).disable();
	} );

	return this;
};


/**
 * Display, or remove the editing form from the display
 *  @param {boolean} show Show (`true`) or hide (`false`)
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.display = function ( show )
{
	if ( show === undefined ) {
		return this.s.displayed;
	}
	return this[ show ? 'open' : 'close' ]();
};


/**
 * Fields which are currently displayed
 *  @return {string[]} Field names that are shown
 */
Editor.prototype.displayed = function ()
{
	return $.map( this.s.fields, function ( field, name ) {
		return field.displayed() ? name : null;
	} );
};


/**
 * Get display controller node
 *
 *  @return {node} Display controller host element
 */
Editor.prototype.displayNode = function ()
{
	return this.s.displayController.node( this );
};


/**
 * Edit a record - show the form, pre-populated with the data that is in the given 
 * DataTables row, that allows the user to enter information for the row to be modified
 * and then subsequently submit that data.
 *  @param {node} items The TR element from the DataTable that is to be edited
 *  @param {boolean} [show=true] Show the form or not.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show the edit form for the first row in the DataTable with a submit button
 *    editor.edit( $('#example tbody tr:eq(0)')[0], 'Edit record', {
 *      "label": "Update",
 *      "fn": function () { this.submit(); }
 *    } );
 *
 *  @example
 *    // Use the title and buttons API methods to show an edit form (this provides
 *    // the same result as example above, but is a different way of achieving it
 *    editor.title( 'Edit record' );
 *    editor.buttons( {
 *      "label": "Update",
 *      "fn": function () { this.submit(); }
 *    } );
 *    editor.edit( $('#example tbody tr:eq(0)')[0] );
 * 
 *  @example
 *    // Automatically submit an edit without showing the user the form
 *    editor.edit( TRnode, null, null, false );
 *    editor.set( 'name', 'Updated name' );
 *    editor.set( 'access', 'Read only' );
 *    editor.submit();
 */
Editor.prototype.edit = function ( items, arg1, arg2, arg3, arg4 )
{
	var that = this;

	// Some other field in inline edit mode?
	if ( this._tidy( function () { that.edit( items, arg1, arg2, arg3, arg4 ); } ) ) {
		return this;
	}

	var argOpts = this._crudArgs( arg1, arg2, arg3, arg4 );

	this._edit( items, this._dataSource( 'fields', items ), 'main' );
	this._assembleMain();
	this._formOptions( argOpts.opts );

	argOpts.maybeOpen();

	return this;
};


/**
 * Enable one or more field inputs, restoring user interaction with the fields.
 *  @param {string|array} name The field name (from the `name` parameter given when
 *   originally setting up the field) to enable, or an array of field names to enable
 *   multiple fields with a single call.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show a 'create' form with buttons which will enable and disable certain fields
 *    editor.create( 'Add new user', [
 *      {
 *        "label": "User name only",
 *        "fn": function () {
 *          this.enable('username');
 *          this.disable( ['first_name', 'last_name'] );
 *        }
 *      }, {
 *        "label": "Name based",
 *        "fn": function () {
 *          this.disable('username');
 *          this.enable( ['first_name', 'last_name'] );
 *        }
 *      }, {
 *        "label": "Submit",
 *        "fn": function () { this.submit(); }
 *      }
 *    );
 */
Editor.prototype.enable = function ( name )
{
	var that = this;

	$.each( this._fieldNames( name ), function ( i, n ) {
		that.field(n).enable();
	} );

	return this;
};


/**
 * Show that a field, or the form globally, is in an error state. Note that
 * errors are cleared on each submission of the form.
 *  @param {string} [name] The name of the field that is in error. If not
 *    given then the global form error display is used.
 *  @param {string} msg The error message to show
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show an error if the field is required
 *    editor.create( 'Add new user', {
 *      "label": "Submit",
 *      "fn": function () {
 *        if ( this.get('username') === '' ) {
 *          this.error( 'username', 'A user name is required' );
 *          return;
 *        }
 *        this.submit();
 *      }
 *    } );
 * 
 *  @example
 *    // Show a field and a global error for a required field
 *    editor.create( 'Add new user', {
 *      "label": "Submit",
 *      "fn": function () {
 *        if ( this.get('username') === '' ) {
 *          this.error( 'username', 'A user name is required' );
 *          this.error( 'The data could not be saved because it is incomplete' );
 *          return;
 *        }
 *        this.submit();
 *      }
 *    } );
 */
Editor.prototype.error = function ( name, msg )
{
	if ( msg === undefined ) {
		// Global error
		this._message( this.dom.formError, name );
	}
	else {
		// Field error
		this.field( name ).error( msg );
	}

	return this;
};


/**
 * Get a field object, configured for a named field, which can then be
 * manipulated through its API. This function effectively acts as a
 * proxy to the field extensions, allowing easy access to the methods
 * for a named field. The methods that are available depend upon the field
 * type plug-in for Editor.
 *
 *   @param {string} name Field name to be obtained
 *   @return {Editor.Field} Field instance
 *
 *   @example
 *     // Update the values available in a select list
 *     editor.field('island').update( [
 *       'Lewis and Harris',
 *       'South Uist',
 *       'North Uist',
 *       'Benbecula',
 *       'Barra'
 *     ] );
 *
 *   @example
 *     // Equivalent calls
 *     editor.field('name').set('John Smith');
 *
 *     // results in the same action as:
 *     editor.set('John Smith');
 */
Editor.prototype.field = function ( name )
{
	var fields = this.s.fields;

	if ( ! fields[name] ) {
		throw 'Unknown field name - '+name;
	}

	return fields[ name ];
};


/**
 * Get a list of the fields that are used by the Editor instance.
 *  @returns {string[]} Array of field names
 * 
 *  @example
 *    // Get current fields and move first item to the end
 *    var fields = editor.fields();
 *    var first = fields.shift();
 *    fields.push( first );
 *    editor.order( fields );
 */
Editor.prototype.fields = function ()
{
	return $.map( this.s.fields, function ( field, name ) {
		return name;
	} );
};

/**
 * Get data object for a file from a table and id
 *
 * @param  {string} name Table name
 * @param  {string|number} id Primary key identifier
 * @return {object} Table information
 */
Editor.prototype.file = _api_file;

/**
 * Get data objects for available files
 *
 * @param  {string} [name] Table name
 * @return {object} Table array
 */
Editor.prototype.files = _api_files;


/**
 * Get the value of a field
 *  @param {string|array} [name] The field name (from the `name` parameter given
 *    when originally setting up the field) to disable. If not given, then an
 *    object of fields is returned, with the value of each field from the
 *    instance represented in the array (the object properties are the field
 *    names). Also an array of field names can be given to get a collection of
 *    data from the form.
 *  @returns {*|object} Value from the named field
 * 
 *  @example
 *    // Client-side validation - check that a field has been given a value 
 *    // before submitting the form
 *    editor.create( 'Add new user', {
 *      "label": "Submit",
 *      "fn": function () {
 *        if ( this.get('username') === '' ) {
 *          this.error( 'username', 'A user name is required' );
 *          return;
 *        }
 *        this.submit();
 *      }
 *    } );
 */
Editor.prototype.get = function ( name )
{
	var that = this;

	if ( ! name ) {
		name = this.fields();
	}

	if ( $.isArray( name ) ) {
		var out = {};

		$.each( name, function (i, n) {
			out[n] = that.field(n).get();
		} );

		return out;
	}

	return this.field(name).get();
};


/**
 * Remove a field from the form display. Note that the field will still be submitted
 * with the other fields in the form, but it simply won't be visible to the user.
 *  @param {string|array} [name] The field name (from the `name` parameter given when
 *   originally setting up the field) to hide or an array of names. If not given then all 
 *   fields are hidden.
 *  @param {boolean} [animate=true] Animate if visible
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show a 'create' record form, but with some fields hidden
 *    editor.hide( 'account_type' );
 *    editor.hide( 'access_level' );
 *    editor.create( 'Add new user', {
 *      "label": "Save",
 *      "fn": function () { this.submit(); }
 *    } );
 *
 *  @example
 *    // Show a single field by hiding all and then showing one
 *    editor.hide();
 *    editor.show('access_type');
 */
Editor.prototype.hide = function ( names, animate )
{
	var that = this;

	$.each( this._fieldNames( names ), function (i, n) {
		that.field( n ).hide( animate );
	} );

	return this;
};


/**
 * Determine if there is an error state in the form, either the form's global
 * error message, or one or more fields.
 *
 * @param {string|array|undefined} [inNames] The field names to check. All
 *   fields checked if undefined.
 * @return {boolean} `true` if there is an error in the form
 */
Editor.prototype.inError = function ( inNames )
{
	// Is there a global error?
	if ( $(this.dom.formError).is(':visible') ) {
		return true;
	}

	// Field specific
	var names = this._fieldNames( inNames );

	for ( var i=0, ien=names.length ; i<ien ; i++ ) {
		if ( this.field( names[i] ).inError() ) {
			return true;
		}
	}

	return false;
};


/**
 * Inline editing for a single field. This method provides a method to allow
 * end users to very quickly edit fields in place. For example, a user could
 * simply click on a cell in a table, the contents of which would be replaced
 * with the editing input field for that cell.
 *
 * @param {string|node|DataTables.Api|cell-selector} cell The cell or field to
 *   be edited (note that for table editing this must be a cell - for standalone
 *   editing it can also be the field name to edit).
 * @param {string} [fieldName] The field name to be edited. This parameter is
 *   optional. If not provided, Editor will attempt to resolve the correct field
 *   from the cell / element given as the first parameter. If it is unable to do
 *   so, it will throw an error.
 * @param {object} [opts] Inline editing options - see the `form-options` type
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.inline = function ( cell, fieldName, opts )
{
	var that = this;

	// Argument shifting
	if ( $.isPlainObject( fieldName ) ) {
		opts = fieldName;
		fieldName = undefined;
	}

	opts = $.extend( {}, this.s.formOptions.inline, opts );

	var editFields = this._dataSource( 'individual', cell, fieldName );
	var node, field;
	var countOuter=0, countInner;
	var closed=false;
	var classes = this.classes.inline;

	// Read the individual cell information from the editFields object
	$.each( editFields, function ( i, editField ) {
		// Only a single row
		if ( countOuter > 0 ) {
			throw 'Cannot edit more than one row inline at a time';
		}

		node = $(editField.attach[0]);

		// Only a single item in that row
		countInner = 0;
		$.each( editField.displayFields, function ( j, f ) {
			if ( countInner > 0 ) {
				throw 'Cannot edit more than one field inline at a time';
			}

			field = f;
			countInner++;
		} );

		countOuter++;

		// If only changed values are to be submitted, then only allow the
		// individual field that we are editing to be edited.
		// This is currently disabled, as I'm not convinced that it is actually
		// useful!
		// if ( opts.submit === 'changed' ) {
		// 	editField.fields = editField.displayFields;
		// }
	} );
	
	// Already in edit mode for this cell?
	if ( $('div.DTE_Field', node).length ) {
		return this;
	}

	// Some other field in inline edit mode?
	if ( this._tidy( function () { that.inline( cell, fieldName, opts ); } ) ) {
		return this;
	}

	// Start a full row edit, but don't display - we will be showing the field
	this._edit( cell, editFields, 'inline' );
	var namespace = this._formOptions( opts );

	var ret = this._preopen( 'inline' );
	if ( ! ret ) {
		return this;
	}

	// Remove from DOM, keeping event handlers, and include text nodes in remove
	var children = node.contents().detach();

	node.append( $(
		'<div class="'+classes.wrapper+'">'+
			'<div class="'+classes.liner+'">'+
				'<div class="DTE_Processing_Indicator"><span/></div>'+
			'</div>'+
			'<div class="'+classes.buttons+'"/>'+
		'</div>'
	) );

	node.find('div.'+classes.liner.replace(/ /g, '.'))
		.append( field.node() )
		.append( this.dom.formError );

	if ( opts.buttons ) {
		// Use prepend for the CSS, so we can float the buttons right
		node.find('div.'+classes.buttons.replace(/ /g, '.')).append( this.dom.buttons );
	}

	this._closeReg( function ( submitComplete ) {
		// Mark that this specific inline edit has closed
		closed = true;

		$(document).off( 'click'+namespace );

		// If there was no submit, we need to put the DOM back as it was. If
		// there was a submit, the write of the new value will set the DOM to
		// how it should be
		if ( ! submitComplete ) {
			node.contents().detach();
			node.append( children );
		}

		// Clear error messages "offline"
		that._clearDynamicInfo();
	} );

	// Submit and blur actions
	setTimeout( function () {
		// If already closed, possibly due to some other aspect of the event
		// that triggered the inline call, don't add the event listener - it
		// isn't needed (and is dangerous)
		if ( closed ) {
			return;
		}

		$(document).on( 'click'+namespace, function ( e ) {
			// Was the click inside or owned by the editing node? If not, then
			// come out of editing mode.

			// andSelf is deprecated in jQ1.8, but we want 1.7 compat
			var back = $.fn.addBack ? 'addBack' : 'andSelf';

			if ( ! field._typeFn( 'owns', e.target ) && 
				 $.inArray( node[0], $(e.target).parents()[ back ]() ) === -1 )
			{
				that.blur();
			}
		} );
	}, 0 );

	this._focus( [ field ], opts.focus );
	this._postopen( 'inline' );

	return this;
};


/**
 * Show an information message for the form as a whole, or for an individual
 * field. This can be used to provide helpful information to a user about an
 * individual field, or more typically the form (for example when deleting
 * a record and asking for confirmation).
 *  @param {string} [name] The name of the field to show the message for. If not
 *    given then a global message is shown for the form
 *  @param {string|function} msg The message to show
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Show a global message for a 'create' form
 *    editor.message( 'Add a new user to the database by completing the fields below' );
 *    editor.create( 'Add new user', {
 *      "label": "Submit",
 *      "fn": function () { this.submit(); }
 *    } );
 * 
 *  @example
 *    // Show a message for an individual field when a 'help' icon is clicked on
 *    $('#user_help').click( function () {
 *      editor.message( 'user', 'The user name is what the system user will login with' );
 *    } );
 */
Editor.prototype.message = function ( name, msg )
{
	if ( msg === undefined ) {
		// Global message
		this._message( this.dom.formInfo, name );
	}
	else {
		// Field message
		this.field( name ).message( msg );
	}

	return this;
};


/**
 * Get which mode of operation the Editor form is in
 *  @return {string} `create`, `edit`, `remove` or `null` if no active state.
 */
Editor.prototype.mode = function ( mode )
{
	if ( ! mode ) {
		return this.s.action;
	}
	
	if ( ! this.s.action ) {
		throw 'Not currently in an editing mode';
	}

	this.s.action = mode;
	return this;
};


/**
 * Get the modifier that was used to trigger the edit or delete action.
 *  @return {*} The identifier that was used for the editing / remove method
 *    called.
 */
Editor.prototype.modifier = function ()
{
	return this.s.modifier;
};


/**
 * Get the values from one or more fields, taking into account multiple data
 * points being edited at the same time.
 *
 * @param  {string|array} fieldNames A single field name or an array of field
 *   names.
 * @return {object} If a string is given as the first parameter an object that
 *   contains the value for each row being edited is returned. If an array is
 *   given, then the object has the field names as the parameter name and the
 *   value is the value object with values for each row being edited.
 */
Editor.prototype.multiGet = function ( fieldNames )
{
	var that = this;

	if ( fieldNames === undefined ) {
		fieldNames = this.fields();
	}

	if ( $.isArray( fieldNames ) ) {
		var out = {};

		$.each( fieldNames, function ( i, name ) {
			out[ name ] = that.field( name ).multiGet();
		} );

		return out;
	}

	// String
	return this.field( fieldNames ).multiGet();
};


/**
 * Set the values for one or more fields, taking into account multiple data
 * points being edited at the same time.
 *
 * @param  {object|string} fieldNames The name of the field to set, or an object
 *   with the field names as the parameters that contains the value object to
 *   set for each field.
 * @param  {*} [val] Value to set if first parameter is given as a string.
 *   Otherwise it is ignored.
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.multiSet = function ( fieldNames, val )
{
	var that = this;

	if ( $.isPlainObject( fieldNames ) && val === undefined ) {
		$.each( fieldNames, function ( name, value ) {
			that.field( name ).multiSet( value );
		} );
	}
	else {
		this.field( fieldNames ).multiSet( val );
	}

	return this;
};


/**
 * Get the container node for an individual field.
 *  @param {string|array} name The field name (from the `name` parameter given
 *   when originally setting up the field) to get the DOM node for.
 *  @return {node|array} Field container node
 * 
 *  @example
 *    // Dynamically add a class to a field's container
 *    $(editor.node( 'account_type' )).addClass( 'account' );
 */
Editor.prototype.node = function ( name )
{
	var that = this;

	if ( ! name ) {
		name = this.order();
	}

	return $.isArray( name ) ?
		$.map( name, function (n) {
			return that.field( n ).node();
		} ) :
		this.field( name ).node();
};


/**
 * Remove a bound event listener to the editor instance. This method provides a 
 * shorthand way of binding jQuery events that would be the same as writing 
 * `$(editor).off(...)` for convenience.
 *  @param {string} name Event name to remove the listeners for - event names are
 *    defined by {@link Editor}.
 *  @param {function} [fn] The function to remove. If not given, all functions which
 *    are assigned to the given event name will be removed.
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Add an event to alert when the form is shown and then remove the listener
 *    // so it will only fire once
 *    editor.on( 'open', function () {
 *      alert('Form displayed!');
 *      editor.off( 'open' );
 *    } );
 */
Editor.prototype.off = function ( name, fn )
{
	$(this).off( this._eventName( name ), fn );

	return this;
};


/**
 * Listen for an event which is fired off by Editor when it performs certain
 * actions. This method provides a shorthand way of binding jQuery events that
 * would be the same as writing  `$(editor).on(...)` for convenience.
 *  @param {string} name Event name to add the listener for - event names are
 *    defined by {@link Editor}.
 *  @param {function} fn The function to run when the event is triggered.
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Log events on the console when they occur
 *    editor.on( 'open', function () { console.log( 'Form opened' ); } );
 *    editor.on( 'close', function () { console.log( 'Form closed' ); } );
 *    editor.on( 'submit', function () { console.log( 'Form submitted' ); } );
 */
Editor.prototype.on = function ( name, fn )
{
	$(this).on( this._eventName( name ), fn );

	return this;
};


/**
 * Listen for a single event event which is fired off by Editor when it performs
 * certain actions. This method provides a shorthand way of binding jQuery
 * events that would be the same as writing  `$(editor).one(...)` for
 * convenience.
 *  @param {string} name Event name to add the listener for - event names are
 *    defined by {@link Editor}.
 *  @param {function} fn The function to run when the event is triggered.
 *  @return {Editor} Editor instance, for chaining
 */
Editor.prototype.one = function ( name, fn )
{
	$(this).one( this._eventName( name ), fn );

	return this;
};


/**
 * Display the main form editor to the end user in the web-browser.
 * 
 * Note that the `close()` method will close any of the three Editor form types
 * (main, bubble and inline), but this method will open only the main type.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Build a 'create' form, but don't display it until some values have
 *    // been set. When done, then display the form.
 *    editor.create( 'Create user', {
 *      "label": "Submit",
 *      "fn": function () { this.submit(); }
 *    }, false );
 *    editor.set( 'name', 'Test user' );
 *    editor.set( 'access', 'Read only' );
 *    editor.open();
 */
Editor.prototype.open = function ()
{
	var that = this;

	// Insert the display elements in order
	this._displayReorder();

	// Define how to do a close
	this._closeReg( function ( submitComplete ) {
		that.s.displayController.close( that, function () {
			that._clearDynamicInfo();
		} );
	} );

	// Run the standard open with common events
	var ret = this._preopen( 'main' );
	if ( ! ret ) {
		return this;
	}

	this.s.displayController.open( this, this.dom.wrapper );
	this._focus(
		$.map( this.s.order, function (name) {
			return that.s.fields[ name ];
		} ),
		this.s.editOpts.focus
	);
	this._postopen( 'main' );

	return this;
};


/**
 * Get or set the ordering of fields, as they are displayed in the form. When used as
 * a getter, the field names are returned in an array, in their current order, and when
 * used as a setting you can alter the field ordering by passing in an array with all
 * field names in their new order.
 * 
 * Note that all fields *must* be included when reordering, and no additional fields can 
 * be added here (use {@link Editor#add} to add more fields). Finally, for setting the 
 * order, you can pass an array of the field names, or give the field names as individual
 * parameters (see examples below).
 *  @param {array|string} [set] Field order to set.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Get field ordering
 *    var order = editor.order();
 * 
 *  @example
 *    // Set the field order
 *    var order = editor.order();
 *    order.unshift( order.pop() ); // move the last field into the first position
 *    editor.order( order );
 * 
 *  @example
 *    // Set the field order as arguments
 *    editor.order( "pupil", "grade", "dept", "exam-board" );
 *
 */
Editor.prototype.order = function ( set /*, ... */ )
{
	if ( !set ) {
		return this.s.order;
	}

	// Allow new layout to be passed in as arguments
	if ( arguments.length && ! $.isArray( set ) ) {
		set = Array.prototype.slice.call(arguments);
	}

	// Sanity check - array must exactly match the fields we have available
	if ( this.s.order.slice().sort().join('-') !== set.slice().sort().join('-') ) {
		throw "All fields, and no additional fields, must be provided for ordering.";
	}

	// Copy the new array into the order (so the reference is maintained)
	$.extend( this.s.order, set );

	this._displayReorder();

	return this;
};


/**
 * Remove (delete) entries from the table. The rows to remove are given as
 * either a single DOM node or an array of DOM nodes (including a jQuery
 * object).
 *  @param {node|array} items The row, or array of nodes, to delete
 *  @param {boolean} [show=true] Show the form or not.
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Delete a given row with a message to let the user know exactly what is
 *    // happening
 *    editor.message( "Are you sure you want to remove this row?" );
 *    editor.remove( row_to_delete, 'Delete row', {
 *      "label": "Confirm",
 *      "fn": function () { this.submit(); }
 *    } );
 * 
 *  @example
 *    // Delete the first row in a table without asking the user for confirmation
 *    editor.remove( '', $('#example tbody tr:eq(0)')[0], null, false );
 *    editor.submit();
 * 
 *  @example
 *    // Delete all rows in a table with a submit button
 *    editor.remove( $('#example tbody tr'), 'Delete all rows', {
 *      "label": "Delete all",
 *      "fn": function () { this.submit(); }
 *    } );
 */
Editor.prototype.remove = function ( items, arg1, arg2, arg3, arg4 )
{
	var that = this;

	// Some other field in inline edit mode?
	if ( this._tidy( function () { that.remove( items, arg1, arg2, arg3, arg4 ); } ) ) {
		return this;
	}

	// Allow a single row node to be passed in to remove, Can't use $.isArray
	// as we also allow array like objects to be passed in (API, jQuery)
	if ( items.length === undefined ) {
		items = [ items ];
	}

	var argOpts = this._crudArgs( arg1, arg2, arg3, arg4 );
	var editFields = this._dataSource( 'fields', items );

	this.s.action = "remove";
	this.s.modifier = items;
	this.s.editFields = editFields;
	this.dom.form.style.display = 'none';

	this._actionClass();

	this._event( 'initRemove', [
		_pluck( editFields, 'node' ),
		_pluck( editFields, 'data' ),
		items
	] );

	this._event( 'initMultiRemove', [
		editFields,
		items
	] );

	this._assembleMain();
	this._formOptions( argOpts.opts );

	argOpts.maybeOpen();

	var opts = this.s.editOpts;
	if ( opts.focus !== null ) {
		$('button', this.dom.buttons).eq( opts.focus ).focus();
	}

	return this;
};


/**
 * Set the value of a field
 *  @param {string|object} name The field name (from the `name` parameter given
 *    when originally setting up the field) to set the value of. If given as an
 *    object the object parameter name will be the value of the field to set and
 *    the value the value to set for the field.
 *  @param {*} [val] The value to set the field to. The format of the value will
 *    depend upon the field type. Not required if the first parameter is given
 *    as an object.
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Set the values of a few fields before then automatically submitting the form
 *    editor.create( null, null, false );
 *    editor.set( 'name', 'Test user' );
 *    editor.set( 'access', 'Read only' );
 *    editor.submit();
 */
Editor.prototype.set = function ( set, val )
{
	var that = this;

	if ( ! $.isPlainObject( set ) ) {
		var o = {};
		o[ set ] = val;
		set = o;
	}

	$.each( set, function (n, v) {
		that.field( n ).set( v );
	} );

	return this;
};


/**
 * Show a field in the display that was previously hidden.
 *  @param {string|array} [names] The field name (from the `name` parameter
 *   given when originally setting up the field) to make visible, or an array of
 *   field names to make visible. If not given all fields are shown.
 *  @param {boolean} [animate=true] Animate if visible
 *  @return {Editor} Editor instance, for chaining
 * 
 *  @example
 *    // Shuffle the fields that are visible, hiding one field and making two
 *    // others visible before then showing the {@link Editor#create} record form.
 *    editor.hide( 'username' );
 *    editor.show( 'account_type' );
 *    editor.show( 'access_level' );
 *    editor.create( 'Add new user', {
 *      "label": "Save",
 *      "fn": function () { this.submit(); }
 *    } );
 *
 *  @example
 *    // Show all fields
 *    editor.show();
 */
Editor.prototype.show = function ( names, animate )
{
	var that = this;

	$.each( this._fieldNames( names ), function (i, n) {
		that.field( n ).show( animate );
	} );

	return this;
};


/**
 * Submit a form to the server for processing. The exact action performed will depend
 * on which of the methods {@link Editor#create}, {@link Editor#edit} or 
 * {@link Editor#remove} were called to prepare the form - regardless of which one is 
 * used, you call this method to submit data.
 *  @param {function} [successCallback] Callback function that is executed once the
 *    form has been successfully submitted to the server and no errors occurred.
 *  @param {function} [errorCallback] Callback function that is executed if the
 *    server reports an error due to the submission (this includes a JSON formatting
 *    error should the error return invalid JSON).
 *  @param {function} [formatdata] Callback function that is passed in the data
 *    that will be submitted to the server, allowing pre-formatting of the data,
 *    removal of data or adding of extra fields.
 *  @param {boolean} [hide=true] When the form is successfully submitted, by default
 *    the form display will be hidden - this option allows that to be overridden.
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Submit data from a form button
 *    editor.create( 'Add new record', {
 *      "label": "Save",
 *      "fn": function () {
 *        this.submit();
 *      }
 *    } );
 *
 *  @example
 *    // Submit without showing the user the form
 *    editor.create( null, null, false );
 *    editor.submit();
 *
 *  @example
 *    // Provide success and error callback methods
 *    editor.create( 'Add new record', {
 *      "label": "Save",
 *      "fn": function () {
 *        this.submit( function () {
 *            alert( 'Form successfully submitted!' );
 *          }, function () {
 *            alert( 'Form  encountered an error :-(' );
 *          }
 *        );
 *      }
 *    } );
 *  
 *  @example
 *    // Add an extra field to the data
 *    editor.create( 'Add new record', {
 *      "label": "Save",
 *      "fn": function () {
 *        this.submit( null, null, function (data) {
 *          data.extra = "Extra information";
 *        } );
 *      }
 *    } );
 *
 *  @example
 *    // Don't hide the form immediately - change the title and then close the form
 *    // after a small amount of time
 *    editor.create( 'Add new record', {
 *      "label": "Save",
 *      "fn": function () {
 *        this.submit( 
 *          function () {
 *            var that = this;
 *            this.title( 'Data successfully added!' );
 *            setTimeout( function () {
 *              that.close();
 *            }, 1000 );
 *          },
 *          null,
 *          null,
 *          false
 *        );
 *      }
 *    } );
 *    
 */
Editor.prototype.submit = function ( successCallback, errorCallback, formatdata, hide )
{
	var
		that = this,
		fields = this.s.fields,
		errorFields = [],
		errorReady = 0,
		sent = false;

	if ( this.s.processing || ! this.s.action ) {
		return this;
	}
	this._processing( true );

	// If there are fields in error, we want to wait for the error notification
	// to be cleared before the form is submitted - errorFields tracks the
	// fields which are in the error state, while errorReady tracks those which
	// are ready to submit
	var send = function () {
		if ( errorFields.length !== errorReady || sent ) {
			return;
		}

		sent = true;
		that._submit( successCallback, errorCallback, formatdata, hide );
	};

	// Remove the global error (don't know if the form is still in an error
	// state!)
	this.error();

	// Count how many fields are in error
	$.each( fields, function ( name, field ) {
		if ( field.inError() ) {
			errorFields.push( name );
		}
	} );

	// Remove the error display
	$.each( errorFields, function ( i, name ) {
		fields[ name ].error('', function () {
			errorReady++;
			send();
		} );
	} );

	send();

	return this;
};


/**
 * Get / set the form template
 * @param  {string|node|jQuery|undefined} set If undefined, treat as a getter,
 *   otherwise set as the template - usually a selector.
 * @return {Editor|string|node|jQuery} Self is a setter, otherwise the template
 */
Editor.prototype.template = function ( set )
{
	if ( set === undefined ) {
		return this.s.template;
	}

	this.s.template = $(set);
	return this;
};


/**
 * Set the title of the form
 *  @param {string|function} title The title to give to the form
 *  @return {Editor} Editor instance, for chaining
 *
 *  @example
 *    // Create an edit display used the title, buttons and edit methods (note that
 *    // this is just an example, typically you would use the parameters of the edit
 *    // method to achieve this.
 *    editor.title( 'Edit record' );
 *    editor.buttons( {
 *      "label": "Update",
 *      "fn": function () { this.submit(); }
 *    } );
 *    editor.edit( TR_to_edit );
 *
 *  @example
 *    // Show a create form, with a timer for the duration that the form is open
 *    editor.create( 'Add new record - time on form: 0s', {
 *      "label": "Save",
 *      "fn": function () { this.submit(); }
 *    } );
 *    
 *    // Add an event to the editor to stop the timer when the display is removed
 *    var runTimer = true;
 *    var timer = 0;
 *    editor.on( 'close', function () {
 *      runTimer = false;
 *      editor.off( 'close' );
 *    } );
 *    // Start the timer running
 *    updateTitle();
 *
 *    // Local function to update the title once per second
 *    function updateTitle() {
 *      editor.title( 'Add new record - time on form: '+timer+'s' );
 *      timer++;
 *      if ( runTimer ) {
 *        setTimeout( function() {
 *          updateTitle();
 *        }, 1000 );
 *      }
 *    }
 */
Editor.prototype.title = function ( title )
{
	var header = $(this.dom.header).children( 'div.'+this.classes.header.content );

	if ( title === undefined ) {
		return header.html();
	}

	if ( typeof title === 'function' ) {
		title = title( this, new DataTable.Api(this.s.table) );
	}

	header.html( title );

	return this;
};


/**
 * Get or set the value of a specific field, or get the value of all fields in
 * the form.
 *
 * @param {string|array} [names] The field name(s) to get or set the value of.
 *   If not given, then the value of all fields will be obtained.
 * @param {*} [value] Value to set
 * @return {Editor|object|*} Editor instance, for chaining if used as a setter,
 *   an object containing the values of the requested fields if used as a
 *   getter with multiple fields requested, or the value of the requested field
 *   if a single field is requested.
 */
Editor.prototype.val = function ( field, value )
{
	if ( value !== undefined || $.isPlainObject( field ) ) {
		return this.set( field, value );
	}

	return this.get( field ); // field can be undefined to get all
};


/*
 * DataTables 1.10 API integration. Provides the ability to control basic Editor
 * aspects from the DataTables API. Full control does of course require use of
 * the Editor API though.
 */
var apiRegister = DataTable.Api.register;


function __getInst( api ) {
	var ctx = api.context[0];
	return ctx.oInit.editor || ctx._editor;
}

// Set sensible defaults for the editing options
function __setBasic( inst, opts, type, plural ) {
	if ( ! opts ) {
		opts = {};
	}

	if ( opts.buttons === undefined ) {
		opts.buttons = '_basic';
	}

	if ( opts.title === undefined ) {
		opts.title = inst.i18n[ type ].title;
	}

	if ( opts.message === undefined ) {
		if ( type === 'remove' ) {
			var confirm = inst.i18n[ type ].confirm;
			opts.message = plural!==1 ? confirm._.replace(/%d/, plural) : confirm['1'];
		}
		else {
			opts.message = '';
		}
	}

	return opts;
}


apiRegister( 'editor()', function () {
	return __getInst( this );
} );

// Row editing
apiRegister( 'row.create()', function ( opts ) {
	// main
	var inst = __getInst( this );
	inst.create( __setBasic( inst, opts, 'create' ) );
	return this;
} );

apiRegister( 'row().edit()', function ( opts ) {
	// main
	var inst = __getInst( this );
	inst.edit( this[0][0], __setBasic( inst, opts, 'edit' ) );
	return this;
} );

apiRegister( 'rows().edit()', function ( opts ) {
	// main
	var inst = __getInst( this );
	inst.edit( this[0], __setBasic( inst, opts, 'edit' ) );
	return this;
} );

apiRegister( 'row().delete()', function ( opts ) {
	// main
	var inst = __getInst( this );
	inst.remove( this[0][0], __setBasic( inst, opts, 'remove', 1 ) );
	return this;
} );

apiRegister( 'rows().delete()', function ( opts ) {
	// main
	var inst = __getInst( this );
	inst.remove( this[0], __setBasic( inst, opts, 'remove', this[0].length ) );
	return this;
} );

apiRegister( 'cell().edit()', function ( type, opts ) {
	// inline or bubble
	if ( ! type ) {
		type = 'inline';
	}
	else if ( $.isPlainObject( type ) ) {
		opts = type;
		type = 'inline';
	}

	__getInst( this )[ type ]( this[0][0], opts );
	return this;
} );

apiRegister( 'cells().edit()', function ( opts ) {
	// bubble only at the moment
	__getInst( this ).bubble( this[0], opts );
	return this;
} );

apiRegister( 'file()', _api_file );
apiRegister( 'files()', _api_files );

// Global listener for file information updates via DataTables' Ajax JSON
$(document).on( 'xhr.dt', function (e, ctx, json) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	if ( json && json.files ) {
		$.each( json.files, function ( name, files ) {
			Editor.files[ name ] = files;
		} );
	}
} );


/**
 * Common error message emitter. This method is not (yet) publicly documented on
 * the Editor site. It might be in future.
 *
 * @param  {string} msg Error message
 * @param  {int}    tn  Tech note link
 */
Editor.error = function ( msg, tn )
{
	throw tn ?
		msg +' For more information, please refer to https://datatables.net/tn/'+tn :
		msg;
};


/**
 * Obtain label / value pairs of data from a data source, be it an array or
 * object, for use in an input that requires label / value pairs such as
 * `select`, `radio` and `checkbox` inputs.
 *
 * A callback function is triggered for each label / value pair found, so the
 * caller can add it to the input as required.
 *
 * @static
 * @param {object|array} An object or array of data to iterate over getting the
 *     label / value pairs.
 * @param {object} props When an array of objects is passed in as the data
 *     source by default the label will be read from the `label` property and
 *     the value from the `value` property of the object. This option can alter
 *     that behaviour.
 * @param {function} fn Callback function. Takes three parameters: the label,
 *      the value and the iterator index.
 */
Editor.pairs = function ( data, props, fn )
{
	var i, ien, dataPoint;

	// Define default properties to read the data from if using an object.
	// The passed in `props` object and override.
	props = $.extend( {
		label: 'label',
		value: 'value'
	}, props );

	if ( $.isArray( data ) ) {
		// As an array, we iterate each item which can be an object or value
		for ( i=0, ien=data.length ; i<ien ; i++ ) {
			dataPoint = data[i];

			if ( $.isPlainObject( dataPoint ) ) {
				fn( 
					dataPoint[ props.value ] === undefined ?
						dataPoint[ props.label ] :
						dataPoint[ props.value ],
					dataPoint[ props.label ],
					i,
					dataPoint.attr // optional - can be undefined
				);
			}
			else {
				fn( dataPoint, dataPoint, i );
			}
		}
	}
	else {
		// As an object the key is the label and the value is the value
		i = 0;

		$.each( data, function ( key, val ) {
			fn( val, key, i );
			i++;
		} );
	}
};


/**
 * Make a string safe to use as a DOM ID. This is primarily for use by field
 * plug-in authors.
 *
 * @static
 * @param {string} String to make safe
 * @param {string} Safe string
 */
Editor.safeId = function ( id )
{
	return id.replace(/\./g, '-');
};


/**
 * Field specific upload method. This can be used to upload a file to the Editor
 * libraries. This method is not (yet) publicly documented on the Editor site.
 * It might be in future.
 *
 * @static
 * @param {Editor} editor The Editor instance operating on
 * @param {object} conf Field configuration object
 * @param {Files} files The file(s) to upload
 * @param {function} progressCallback Upload progress callback
 * @param {function} completeCallback Callback function for once the file has
 *     been uploaded
 */
Editor.upload = function ( editor, conf, files, progressCallback, completeCallback )
{
	var reader = new FileReader();
	var counter = 0;
	var ids = [];
	var generalError = 'A server error occurred while uploading the file';

	// Clear any existing errors, as the new upload might not be in error
	editor.error( conf.name, '' );

	progressCallback( conf, conf.fileReadText || "<i>Uploading file</i>" );

	reader.onload = function ( e ) {
		var data = new FormData();
		var ajax;

		data.append( 'action', 'upload' );
		data.append( 'uploadField', conf.name );
		data.append( 'upload', files[ counter ] );

		if ( conf.ajaxData ) {
			conf.ajaxData( data );
		}

		if ( conf.ajax ) {
			ajax = conf.ajax;
		}
		else if ( $.isPlainObject( editor.s.ajax ) ) {
			ajax = editor.s.ajax.upload ?
				editor.s.ajax.upload :
				editor.s.ajax;
		}
		else if ( typeof editor.s.ajax === 'string' ) {
			ajax = editor.s.ajax;
		}

		if ( ! ajax ) {
			throw 'No Ajax option specified for upload plug-in';
		}

		if ( typeof ajax === 'string' ) {
			ajax = { url: ajax };
		}

		// Use preSubmit to stop form submission during an upload, since the
		// value won't be known until that point.
		var submit = false;
		editor
			.on( 'preSubmit.DTE_Upload', function () {
				submit = true;
				return false;
			} );

		// Handle the case when the ajax data is given as a function
		if ( typeof ajax.data === 'function' ) {
			var d = {};
			var ret = ajax.data( d );

			// Allow the return to be used, or the object passed in
			if ( ret !== undefined ) {
				d = ret;
			}

			$.each( d, function ( key, value ) {
				data.append( key, value );
			} );
		}

		$.ajax( $.extend( {}, ajax, {
			type: 'post',
			data: data,
			dataType: 'json',
			contentType: false,
			processData: false,
			xhr: function () {
				var xhr = $.ajaxSettings.xhr();

				if ( xhr.upload ) {
					xhr.upload.onprogress = function ( e ) {
						if ( e.lengthComputable ) {
							var percent = (e.loaded/e.total*100).toFixed(0)+"%";

							progressCallback( conf, files.length === 1 ?
								percent :
								counter+':'+files.length+' '+percent
							);
						}
					};
					xhr.upload.onloadend = function ( e ) {
						progressCallback( conf );
					};
				}

				return xhr;
			},
			success: function ( json ) {
				editor.off( 'preSubmit.DTE_Upload' );
				editor._event( 'uploadXhrSuccess', [ conf.name, json ] );

				if ( json.fieldErrors && json.fieldErrors.length ) {
					var errors = json.fieldErrors;

					for ( var i=0, ien=errors.length ; i<ien ; i++ ) {
						editor.error( errors[i].name, errors[i].status );
					}
				}
				else if ( json.error ) {
					editor.error( json.error );
				}
				else if ( ! json.upload || ! json.upload.id ) {
					editor.error( conf.name, generalError );
				}
				else {
					if ( json.files ) {
						// Loop over the tables that are defined
						$.each( json.files, function ( table, files ) {
							if ( ! Editor.files[ table ] ) {
								Editor.files[ table ] = {};
							}
							$.extend( Editor.files[ table ], files );
						} );
					}

					ids.push( json.upload.id );

					if ( counter < files.length-1 ) {
						counter++;
						reader.readAsDataURL( files[counter] );
					}
					else {
						completeCallback.call( editor, ids );
						
						if ( submit ) {
							editor.submit();
						}
					}
				}
			},
			error: function ( xhr ) {
				editor._event( 'uploadXhrError', [ conf.name, xhr ] );
				editor.error( conf.name, generalError );
			}
		} ) );
	};

	reader.readAsDataURL( files[0] );
};
/**
 * Editor constructor - take the developer configuration and apply it to the instance.
 *  @param {object} init The initialisation options provided by the developer - see
 *    {@link Editor.defaults} for a full list of options.
 *  @private
 */
Editor.prototype._constructor = function(init) {
    init = $.extend(true, {}, Editor.defaults, init);
    this.s = $.extend(true, {}, Editor.models.settings, {
        table: init.domTable || init.table,
        dbTable: init.dbTable || null, // legacy
        ajaxUrl: init.ajaxUrl,
        ajax: init.ajax,
        idSrc: init.idSrc,
        dataSource: init.domTable || init.table ?
            Editor.dataSources.dataTable : Editor.dataSources.html,
        formOptions: init.formOptions,
        legacyAjax: init.legacyAjax,
        template: init.template ?
            $(init.template).detach() : null
    });
    this.classes = $.extend(true, {}, Editor.classes);
    this.i18n = init.i18n;

    // Increment the unique counter for the next instance
    Editor.models.settings.unique++;

    var that = this;
    var classes = this.classes;

    this.dom = {
        "wrapper": $(
            '<div class="' + classes.wrapper + '">' +
            '<div data-dte-e="processing" class="' + classes.processing.indicator + '"><span/></div>' +
            '<div data-dte-e="body" class="' + classes.body.wrapper + '">' +
            '<div data-dte-e="body_content" class="' + classes.body.content + '"/>' +
            '</div>' +
            '<div data-dte-e="foot" class="' + classes.footer.wrapper + '">' +
            '<div class="' + classes.footer.content + '"/>' +
            '</div>' +
            '</div>'
        )[0],
        "form": $(
            '<form data-dte-e="form" class="' + classes.form.tag + '">' +
            '<div data-dte-e="form_content" class="' + classes.form.content + '"/>' +
            '</form>'
        )[0],
        "formError": $('<div data-dte-e="form_error" class="' + classes.form.error + '"/>')[0],
        "formInfo": $('<div data-dte-e="form_info" class="' + classes.form.info + '"/>')[0],
        "header": $('<div data-dte-e="head" class="' + classes.header.wrapper + '"><div class="' + classes.header.content + '"/></div>')[0],
        "buttons": $('<div data-dte-e="form_buttons" class="' + classes.form.buttons + '"/>')[0]
    };

    // Customise the TableTools buttons with the i18n settings - worth noting that
    // this could easily be done outside the Editor instance, but this makes things
    // a bit easier to understand and more cohesive. Also worth noting that when
    // there are two or more Editor instances, the init sequence should be
    // Editor / DataTables, Editor / DataTables etc, since the value of these button
    // instances matter when you create the TableTools buttons for the DataTable.
    if ($.fn.dataTable.TableTools) {
        var ttButtons = $.fn.dataTable.TableTools.BUTTONS;
        var i18n = this.i18n;

        $.each(['create', 'edit', 'remove'], function(i, val) {
            ttButtons['editor_' + val].sButtonText = i18n[val].button;
        });
    }

    // Bind callback methods
    $.each(init.events, function(evt, fn) {
        that.on(evt, function() {
            // When giving events in the constructor the event argument was not
            // given in 1.2-, so we remove it here. This is solely for
            // backwards compatibility as the events in the initialisation are
            // not documented in 1.3+.
            var args = Array.prototype.slice.call(arguments);
            args.shift();
            fn.apply(that, args);
        });
    });

    // Cache the DOM nodes
    var dom = this.dom;
    var wrapper = dom.wrapper;
    dom.formContent = _editor_el('form_content', dom.form)[0];
    dom.footer = _editor_el('foot', wrapper)[0];
    dom.body = _editor_el('body', wrapper)[0];
    dom.bodyContent = _editor_el('body_content', wrapper)[0];
    dom.processing = _editor_el('processing', wrapper)[0];

    // Add any fields which are given on initialisation
    if (init.fields) {
        this.add(init.fields);
    }

    $(document)
        .on('init.dt.dte' + this.s.unique, function(e, settings, json) {
            // Attempt to attach to a DataTable automatically when the table is
            // initialised
            if (that.s.table && settings.nTable === $(that.s.table).get(0)) {
                settings._editor = that;
            }
        })
        .on('xhr.dt.dte' + this.s.unique, function(e, settings, json) {
            // Automatically update fields which have a field name defined in
            // the returned json - saves an `initComplete` for the user
            if (json && that.s.table && settings.nTable === $(that.s.table).get(0)) {
                that._optionsUpdate(json);
            }
        });

    // Prep the display controller
    this.s.displayController = Editor.display[init.display].init(this);

    this._event('initComplete', []);
};
/*global __inlineCounter*/

/**
 * Set the class on the form to relate to the action that is being performed.
 * This allows styling to be applied to the form to reflect the state that
 * it is in.
 *
 * @private
 */
Editor.prototype._actionClass = function ()
{
	var classesActions = this.classes.actions;
	var action = this.s.action;
	var wrapper = $(this.dom.wrapper);

	wrapper.removeClass( [classesActions.create, classesActions.edit, classesActions.remove].join(' ') );

	if ( action === "create" ) {
		wrapper.addClass( classesActions.create );
	}
	else if ( action === "edit" ) {
		wrapper.addClass( classesActions.edit );
	}
	else if ( action === "remove" ) {
		wrapper.addClass( classesActions.remove );
	}
};


/**
 * Create an Ajax request in the same style as DataTables 1.10, with full
 * backwards compatibility for Editor 1.2.
 *
 * @param  {object} data Data to submit
 * @param  {function} success Success callback
 * @param  {function} error Error callback
 * @param  {object} submitParams Submitted data
 * @private
 */
Editor.prototype._ajax = function ( data, success, error, submitParams )
{
	var that = this;
	var action = this.s.action;
	var thrown;
	var opts = {
		type:     'POST',
		dataType: 'json',
		data:     null,
		error:    [ function ( xhr, text, err ) {
			thrown = err;
		} ],
		success:  [],
		complete: [ function ( xhr, text ) {
			// Use `complete` rather than `success` so that all status codes are
			// caught and can return valid JSON (useful when working with REST
			// services).
			var json = null;

			if ( xhr.status === 204 || xhr.responseText === 'null' ) {
				json = {};
			}
			else {
				try {
					// jQuery 1.12 or newer for responseJSON, but its the only
					// way to get the JSON from a JSONP. So if you want to use
					// JSONP with Editor you have to use jQuery 1.12+.
					json = xhr.responseJSON ?
						xhr.responseJSON :
						$.parseJSON( xhr.responseText );
				}
				catch (e) {}
			}

			if ( $.isPlainObject( json ) || $.isArray( json ) ) {
				success( json, xhr.status >= 400, xhr );
			}
			else {
				error( xhr, text, thrown );
			}
		} ]
	};
	var a;
	var ajaxSrc = this.s.ajax || this.s.ajaxUrl;
	var id = action === 'edit' || action === 'remove' ?
		_pluck( this.s.editFields, 'idSrc' ) :
		null;

	if ( $.isArray( id ) ) {
		id = id.join(',');
	}

	// Get the correct object for rest style
	if ( $.isPlainObject( ajaxSrc ) && ajaxSrc[ action ] ) {
		ajaxSrc = ajaxSrc[ action ];
	}

	if ( $.isFunction( ajaxSrc ) ) {
		// As a function, execute it, passing in the required parameters
		var uri = null;
		var method = null;

		// If the old style ajaxSrc is given, we need to process it for
		// backwards compatibility with 1.2-. Unfortunate otherwise this would
		// be a very simply function!
		if ( this.s.ajaxUrl ) {
			var url = this.s.ajaxUrl;

			if ( url.create ) {
				uri = url[ action ];
			}

			if ( uri.indexOf(' ') !== -1 ) {
				a = uri.split(' ');
				method = a[0];
				uri = a[1];
			}

			uri = uri.replace( /_id_/, id );
		}
		
		ajaxSrc( method, uri, data, success, error );
		return;
	}
	else if ( typeof ajaxSrc === 'string' ) {
		// As a string it gives the URL. For backwards compatibility it can also
		// give the method.
		if ( ajaxSrc.indexOf(' ') !== -1 ) {
			a = ajaxSrc.split(' ');
			opts.type = a[0];
			opts.url = a[1];
		}
		else {
			opts.url = ajaxSrc;
		}
	}
	else {
		// As an object, we extend the Editor defaults - with the exception of
		// the error and complete functions which get added in so the user can
		// specify their own in addition to ours
		var optsCopy = $.extend( {}, ajaxSrc || {} );

		if ( optsCopy.complete ) {
			opts.complete.unshift( optsCopy.complete );
			delete optsCopy.complete;
		}

		if ( optsCopy.error ) {
			opts.error.unshift( optsCopy.error );
			delete optsCopy.error;
		}

		opts = $.extend( {}, opts, optsCopy );
	}

	// URL macros
	opts.url = opts.url.replace( /_id_/, id );

	// Data processing option like in DataTables
	if ( opts.data ) {
		var newData = $.isFunction( opts.data ) ?
			opts.data( data ) :  // fn can manipulate data or return an object
			opts.data;           // object or array to merge

		// If the function returned something, use that alone
		data = $.isFunction( opts.data ) && newData ?
			newData :
			$.extend( true, data, newData );
	}

	opts.data = data;

	// If a DELETE method is used there are a number of servers which will
	// reject the request if it has a body. So we need to append to the URL.
	//
	// http://stackoverflow.com/questions/15088955
	// http://bugs.jquery.com/ticket/11586
	if ( opts.type === 'DELETE' && (opts.deleteBody === undefined || opts.deleteBody === true) ) {
		var params = $.param( opts.data );

		opts.url += opts.url.indexOf('?') === -1 ?
			'?'+params :
			'&'+params;

		delete opts.data;
	}

	// Finally, make the ajax call
	$.ajax( opts );
};


/**
 * Create the DOM structure from the source elements for the main form.
 * This is required since the elements can be moved around for other form types
 * (bubble).
 *
 * @private
 */
Editor.prototype._assembleMain = function ()
{
	var dom = this.dom;

	$(dom.wrapper)
		.prepend( dom.header );

	$(dom.footer)
		.append( dom.formError )
		.append( dom.buttons );

	$(dom.bodyContent)
		.append( dom.formInfo )
		.append( dom.form );
};


/**
 * Blur the editing window. A blur is different from a close in that it might
 * cause either a close or the form to be submitted. A typical example of a
 * blur would be clicking on the background of the bubble or main editing forms
 * - i.e. it might be a close, or it might submit depending upon the
 * configuration, while a click on the close box is a very definite close.
 *
 * @private
 */
Editor.prototype._blur = function ()
{
	var opts = this.s.editOpts;
	var onBlur = opts.onBlur;

	if ( this._event( 'preBlur' ) === false ) {
		return;
	}

	if ( typeof onBlur === 'function' ) {
		onBlur( this );
	}
	else if ( onBlur === 'submit' ) {
		this.submit();
	}
	else if ( onBlur === 'close' ) {
		this._close();
	}
};


/**
 * Clear all of the information that might have been dynamically set while
 * the form was visible - specifically errors and dynamic messages
 *
 * @private
 */
Editor.prototype._clearDynamicInfo = function ()
{
	// Can be triggered due to a destroy if the editor is open
	if ( ! this.s ) {
		return;
	}

	var errorClass = this.classes.field.error;
	var fields = this.s.fields;

	$('div.'+errorClass, this.dom.wrapper).removeClass( errorClass );

	$.each( fields, function (name, field) {
		field
			.error('')
			.message('');
	} );

	this
		.error('')
		.message('');
};


/**
 * Close an editing display, firing callbacks and events as needed
 *
 * @param  {function} submitComplete Function to call after the preClose event
 * @private
 */
Editor.prototype._close = function ( submitComplete )
{
	// Allow preClose event to cancel the opening of the display
	if ( this._event( 'preClose' ) === false ) {
		return;
	}

	if ( this.s.closeCb ) {
		this.s.closeCb( submitComplete );
		this.s.closeCb = null;
	}

	if ( this.s.closeIcb ) {
		this.s.closeIcb();
		this.s.closeIcb = null;
	}

	// Remove focus control
	$('body').off( 'focus.editor-focus' );

	this.s.displayed = false;
	this._event( 'close' );
};


/**
 * Register a function to be called when the editing display is closed. This is
 * used by function that create the editing display to tidy up the display on
 * close - for example removing event handlers to prevent memory leaks.
 *
 * @param  {function} fn Function to call on close
 * @private
 */
Editor.prototype._closeReg = function ( fn )
{
	this.s.closeCb = fn;
};


/**
 * Argument shifting for the create(), edit() and remove() methods. In Editor
 * 1.3 the preferred form of calling those three methods is with just two
 * parameters (one in the case of create() - the id and the show flag), while in
 * previous versions four / three parameters could be passed in, including the
 * buttons and title options. In 1.3 the chaining API is preferred, but we want
 * to support the old form as well, so this function is provided to perform
 * that argument shifting, common to all three.
 *
 * @private
 */
Editor.prototype._crudArgs = function ( arg1, arg2, arg3, arg4 )
{
	var that = this;
	var title;
	var buttons;
	var show;
	var opts;

	if ( $.isPlainObject( arg1 ) ) {
		// Form options passed in as the first option
		opts = arg1;
	}
	else if ( typeof arg1 === 'boolean' ) {
		// Show / hide passed in as the first option - form options second
		show = arg1;
		opts = arg2; // can be undefined
	}
	else {
		// Old style arguments
		title = arg1; // can be undefined
		buttons = arg2; // can be undefined
		show = arg3; // can be undefined
		opts = arg4; // can be undefined
	}

	// If all undefined, then fall into here
	if ( show === undefined ) {
		show = true;
	}

	if ( title ) {
		that.title( title );
	}

	if ( buttons ) {
		that.buttons( buttons );
	}

	return {
		opts: $.extend( {}, this.s.formOptions.main, opts ),
		maybeOpen: function () {
			if ( show ) {
				that.open();
			}
		}
	};
};


/**
 * Execute the data source abstraction layer functions. This is simply a case
 * of executing the function with the Editor scope, passing in the remaining
 * parameters.
 *
 * @param {string) name Function name to execute
 * @private
 */
Editor.prototype._dataSource = function ( name /*, ... */ )
{
	// Remove the name from the arguments list, so the rest can be passed
	// straight into the field type
	var args = Array.prototype.slice.call( arguments );
	args.shift();

	var fn = this.s.dataSource[ name ];
	if ( fn ) {
		return fn.apply( this, args );
	}
};


/**
 * Insert the fields into the DOM, in the correct order
 *
 * @private
 */
Editor.prototype._displayReorder = function ( includeFields )
{
	var that = this;
	var formContent = $(this.dom.formContent);
	var fields = this.s.fields;
	var order = this.s.order;
	var template = this.s.template;
	var mode = this.s.mode || 'main';

	if ( includeFields ) {
		this.s.includeFields = includeFields;
	}
	else {
		includeFields = this.s.includeFields;
	}

	// Empty before adding in the required fields
	formContent.children().detach();

	$.each( order, function (i, fieldOrName) {
		var name = fieldOrName instanceof Editor.Field ?
			fieldOrName.name() :
			fieldOrName;

		if ( that._weakInArray( name, includeFields ) !== -1 ) {
			if ( template && mode === 'main' ) {
				template.find('editor-field[name="'+name+'"]').after(
					fields[ name ].node()
				);

				template.find( '[data-editor-template="'+name+'"]').append(
					fields[ name ].node()
				);
			}
			else {
				formContent.append( fields[ name ].node() );
			}
		}
	} );

	if ( template && mode === 'main' ) {
		template.appendTo( formContent );
	}

	this._event( 'displayOrder', [
		this.s.displayed,
		this.s.action,
		formContent
	] );
};


/**
 * Generic editing handler. This can be called by the three editing modes (main,
 * bubble and inline) to configure Editor for a row edit, and fire the required
 * events to ensure that the editing interfaces all provide a common API.
 *
 * @param {*} rows Identifier for the item(s) to be edited
 * @param {string} type Editing type - for the initEdit event
 * @private
 */
Editor.prototype._edit = function ( items, editFields, type )
{
	var that = this;
	var fields = this.s.fields;
	var usedFields = [];
	var includeInOrder;
	var editData = {};

	this.s.editFields = editFields;
	this.s.editData = editData;
	this.s.modifier = items;
	this.s.action = "edit";
	this.dom.form.style.display = 'block';
	this.s.mode = type;

	this._actionClass();

	// Setup the field values for editing
	$.each( fields, function ( name, field ) {
		field.multiReset();
		includeInOrder = true;
		editData[ name ] = {};

		$.each( editFields, function ( idSrc, edit ) {
			if ( edit.fields[ name ] ) {
				var val = field.valFromData( edit.data );

				// Save the set data values so we can decided in submit if data has changed
				editData[ name ][ idSrc ] = val;

				field.multiSet( idSrc, val !== undefined ?
					val :
					field.def()
				);

				// If there is an displayFields object, we need to know if this
				// field is present in it or not. If not, then the field isn't
				// displayed
				if ( edit.displayFields && ! edit.displayFields[ name ] ) {
					includeInOrder = false;
				}
			}
		} );

		// If the field is used, then add it to the fields to be shown
		if ( field.multiIds().length !== 0 && includeInOrder ) {
			usedFields.push( name );
		}
	} );

	// Remove the fields that are not required from the display
	var currOrder = this.order().slice();

	for ( var i=currOrder.length-1 ; i >= 0 ; i-- ) {
		// Use `toString()` to convert numbers to strings, since usedFields
		// contains strings (object property names)
		if ( $.inArray( currOrder[i].toString(), usedFields ) === -1 ) {
			currOrder.splice( i, 1 );
		}
	}

	this._displayReorder( currOrder );

	// Events
	this._event( 'initEdit', [
		_pluck( editFields, 'node' )[0],
		_pluck( editFields, 'data' )[0],
		items,
		type
	] );

	this._event( 'initMultiEdit', [
		editFields,
		items,
		type
	] );
};


/**
 * Fire callback functions and trigger events.
 *
 * @param {string|array} trigger Name(s) of the jQuery custom event to trigger
 * @param {array} args Array of arguments to pass to the triggered event
 * @return {*} Return from the event
 * @private
 */
Editor.prototype._event = function ( trigger, args )
{
	if ( ! args ) {
		args = [];
	}

	// Allow an array to be passed in for the trigger to fire multiple events
	if ( $.isArray( trigger ) ) {
		for ( var i=0, ien=trigger.length ; i<ien ; i++ ) {
			this._event( trigger[i], args );
		}
	}
	else {
		var e = $.Event( trigger );

		$(this).triggerHandler( e, args );

		return e.result;
	}
};


/**
 * 'Modernise' event names, from the old style `on[A-Z]` names to camelCase.
 * This is done to provide backwards compatibility with Editor 1.2- event names.
 * The names themselves were updated for consistency with DataTables.
 *
 * @param {string} Event name to modernise
 * @return {string} String with new event name structure
 * @private
 */
Editor.prototype._eventName = function ( input )
{
	var name;
	var names = input.split( ' ' );

	for ( var i=0, ien=names.length ; i<ien ; i++ ) {
		name = names[i];

		// Strip the 'on' part and lowercase the first character
		var onStyle = name.match(/^on([A-Z])/);
		if ( onStyle ) {
			name = onStyle[1].toLowerCase() + name.substring( 3 );
		}

		names[i] = name;
	}

	return names.join( ' ' );
};


/**
 * Find a field from a DOM node. All children are searched.
 *
 * @param  {node} node DOM node to search for
 * @return {Field}     Field instance
 */
Editor.prototype._fieldFromNode = function ( node )
{
	var foundField = null;

	$.each( this.s.fields, function ( name, field ) {
		if ( $( field.node() ).find( node ).length ) {
			foundField = field;
		}
	} );

	return foundField;
};


/**
 * Convert a field name input parameter to an array of field names.
 *
 * Many of the API methods provide the ability to pass `undefined` a string or
 * array of strings to identify fields. This method harmonises that.
 *
 * @param  {array|string} [fieldNames] Field names to get
 * @return {array}                     Field names
 * @private
 */
Editor.prototype._fieldNames = function ( fieldNames )
{
	if ( fieldNames === undefined ) {
		return this.fields();
	}
	else if ( ! $.isArray( fieldNames ) ) {
		return [ fieldNames ];
	}

	return fieldNames;
};


/**
 * Focus on a field. Providing the logic to allow complex focus expressions
 *
 * @param {array} fields Array of Field instances or field names for the fields
 *     that are shown
 * @param {null|string|integer} focus Field identifier to focus on
 * @private
 */
Editor.prototype._focus = function ( fieldsIn, focus )
{
	var that = this;
	var field;
	var fields = $.map( fieldsIn, function ( fieldOrName ) {
		return typeof fieldOrName === 'string' ?
			that.s.fields[ fieldOrName ] :
			fieldOrName;
	} );

	if ( typeof focus === 'number' ) {
		field = fields[ focus ];
	}
	else if ( focus ) {
		if ( focus.indexOf( 'jq:' ) === 0 ) {
			field = $('div.DTE '+focus.replace(/^jq:/, ''));
		}
		else {
			field = this.s.fields[ focus ];
		}
	}

	this.s.setFocus = field;

	if ( field ) {
		field.focus();
	}
};


/**
 * Form options - common function so all editing methods can provide the same
 * basic options, DRY.
 *
 * @param {object} opts Editing options. See model.formOptions
 * @private
 */
Editor.prototype._formOptions = function ( opts )
{
	var that = this;
	var inlineCount = __inlineCounter++;
	var namespace = '.dteInline'+inlineCount;

	// Backwards compatibility with 1.4
	if ( opts.closeOnComplete !== undefined ) {
		opts.onComplete = opts.closeOnComplete ? 'close' : 'none';
	}

	if ( opts.submitOnBlur !== undefined ) {
		opts.onBlur = opts.submitOnBlur ? 'submit' : 'close';
	}

	if ( opts.submitOnReturn !== undefined ) {
		opts.onReturn = opts.submitOnReturn ? 'submit' : 'none';
	}

	if ( opts.blurOnBackground !== undefined ) {
		opts.onBackground = opts.blurOnBackground ? 'blur' : 'none';
	}

	this.s.editOpts = opts;

	// When submitting by Ajax we don't want to close a form that has been
	// opened during the ajax request, so we keep a count of the form opening
	this.s.editCount = inlineCount;

	if ( typeof opts.title === 'string' || typeof opts.title === 'function' ) {
		this.title( opts.title );
		opts.title = true;
	}

	if ( typeof opts.message === 'string' || typeof opts.message === 'function' ) {
		this.message( opts.message );
		opts.message = true;
	}

	if ( typeof opts.buttons !== 'boolean' ) {
		this.buttons( opts.buttons );
		opts.buttons = true;
	}

	$(document).on( 'keydown'+namespace, function ( e ) {
		var el = $(document.activeElement);

		if ( e.keyCode === 13 && that.s.displayed ) { // return
			var field = that._fieldFromNode( el );

			// Allow the field plug-in to say if we can submit or not
			if ( field && typeof field.canReturnSubmit === 'function' && field.canReturnSubmit( el ) ) {
				if ( opts.onReturn === 'submit' ) {
					e.preventDefault();
					that.submit();
				}
				else if ( typeof opts.onReturn === 'function' ) {
					e.preventDefault();
					opts.onReturn( that );
				}
			}
		}
		else if ( e.keyCode === 27 ) { // esc
			e.preventDefault();

			if ( typeof opts.onEsc === 'function' ) {
				opts.onEsc( that );
			}
			else if ( opts.onEsc === 'blur' ) {
				that.blur();
			}
			else if ( opts.onEsc === 'close' ) {
				that.close();
			}
			else if ( opts.onEsc === 'submit' ) {
				that.submit();
			}
		}
		else if ( el.parents('.DTE_Form_Buttons').length ) {
			if ( e.keyCode === 37 ) { // left
				el.prev( 'button' ).focus();
			}
			else if ( e.keyCode === 39 ) { // right
				el.next( 'button' ).focus();
			}
		}
	} );

	this.s.closeIcb = function () {
		$(document).off( 'keydown'+namespace );
	};

	return namespace;
};


/**
 * Convert from the 1.5+ data interchange format to the 1.4- format if suitable.
 *
 * @param  {string} direction 'send' or 'receive'
 * @param  {string} action    CRUD action
 * @param  {object} data      Data object to transform
 * @private
 */
Editor.prototype._legacyAjax = function ( direction, action, data )
{
	if ( ! this.s.legacyAjax || ! data ) {
		return;
	}

	if ( direction === 'send' ) {
		if ( action === 'create' || action === 'edit' ) {
			var id;

			$.each( data.data, function ( rowId, values ) {
				if ( id !== undefined ) {
					throw 'Editor: Multi-row editing is not supported by the legacy Ajax data format';
				}

				id = rowId;
			} );

			data.data = data.data[ id ];

			if ( action === 'edit' ) {
				data.id = id;
			}
		}
		else {
			data.id = $.map( data.data, function ( values, id ) {
				return id;
			} );

			delete data.data;
		}
	}
	else {
		if ( ! data.data && data.row ) {
			// 1.4 libraries retuned data in the `row` property
			data.data = [ data.row ];
		}
		else if ( ! data.data ) {
			// 1.4- allowed data not to be returned - 1.5 requires it
			data.data = [];
		}
	}
};


/**
 * Update the field options from a JSON data source
 *
 * @param  {object} json JSON object from the server
 * @private
 */
Editor.prototype._optionsUpdate = function ( json )
{
	var that = this;

	if ( json.options ) {
		$.each( this.s.fields, function (name, field) {
			if ( json.options[ name ] !== undefined ) {
				var fieldInst = that.field( name );

				if ( fieldInst && fieldInst.update ) {
					fieldInst.update( json.options[ name ] );
				}
			}
		} );
	}
};


/**
 * Show a message in the form. This can be used for error messages or dynamic
 * messages (information display) as the structure for each is basically the
 * same. This method will take into account if the form is visible or not - if
 * so then the message is shown with an effect for the end user, otherwise
 * it is just set immediately.
 *
 * @param {element} el The field display node to use
 * @param {string|function} msg The message to show
 * @private
 */
Editor.prototype._message = function ( el, msg )
{
	if ( typeof msg === 'function' ) {
		msg = msg( this, new DataTable.Api(this.s.table) );
	}

	el = $(el);

	if ( ! msg && this.s.displayed ) {
		// Clear the message with visual effect since the form is visible
		el
			.stop()
			.fadeOut( function () {
				el.html( '' );
			} );
	}
	else if ( ! msg ) {
		// Clear the message without visual effect
		el
			.html( '' )
			.css('display', 'none');
	}
	else if ( this.s.displayed ) {
		// Show the message with visual effect
		el
			.stop()
			.html( msg )
			.fadeIn();
	}
	else {
		// Show the message without visual effect
		el
			.html( msg )
			.css('display', 'block');
	}
};


/**
 * Update the multi-value information display to not show redundant information
 *
 * @private
 */
Editor.prototype._multiInfo = function ()
{
	var fields = this.s.fields;
	var include = this.s.includeFields;
	var show = true;
	var state;

	if ( ! include ) {
		return;
	}

	for ( var i=0, ien=include.length ; i<ien ; i++ ) {
		var field = fields[ include[i] ];
		var multiEditable = field.multiEditable();

		if ( field.isMultiValue() && multiEditable && show ) {
			// Multi-row editable. Only show first message
			state = true;
			show = false;
		}
		else if ( field.isMultiValue() && ! multiEditable ) {
			// Not multi-row editable. Always show message
			state = true;
		}
		else {
			state = false;
		}

		fields[ include[i] ].multiInfoShown( state );
	}
};


/**
 * Common display editing form method called by all editing methods after the
 * form has been configured and displayed. This is to ensure all fire the same
 * events.
 *
 * @param  {string} Editing type
 * @return {boolean} `true`
 * @private
 */
Editor.prototype._postopen = function ( type )
{
	var that = this;
	var focusCapture = this.s.displayController.captureFocus;
	if ( focusCapture === undefined ) {
		focusCapture = true;
	}

	$(this.dom.form)
		.off( 'submit.editor-internal' )
		.on( 'submit.editor-internal', function (e) {
			e.preventDefault();
		} );

	// Focus capture - when the Editor form is shown we capture the browser's
	// focus action. Without doing this is would result in the user being able
	// to control items under the Editor display - triggering actions that
	// shouldn't be possible while the editing is shown.
	if ( focusCapture && (type === 'main' || type === 'bubble') ) {
		$('body').on( 'focus.editor-focus', function () {
			if ( $(document.activeElement).parents('.DTE').length === 0 &&
			     $(document.activeElement).parents('.DTED').length === 0
			) {
				if ( that.s.setFocus ) {
					that.s.setFocus.focus();
				}
			}
		} );
	}

	this._multiInfo();

	this._event( 'open', [type, this.s.action] );

	return true;
};


/**
 * Common display editing form method called by all editing methods before the
 * form has been configured and displayed. This is to ensure all fire the same
 * events.
 *
 * @param  {string} Editing type
 * @return {boolean} `false` if the open is cancelled by the preOpen event,
 *   otherwise `true`
 * @private
 */
Editor.prototype._preopen = function ( type )
{
	// Allow preOpen event to cancel the opening of the display
	if ( this._event( 'preOpen', [type, this.s.action] ) === false ) {
		// Tidy- this would normally be done on close, but we never get that far
		this._clearDynamicInfo();
		this._event( 'cancelOpen', [type, this.s.action] );

		// inline and bubble methods cannot be opened using `open()`, they
		// have to be called again, so we need to clean up the event
		// listener added by _formOptions
		if ( (this.s.mode === 'inline' || this.s.mode === 'bubble') && this.s.closeIcb ) {
			this.s.closeIcb();
		}

		this.s.closeIcb = null;

		return false;
	}

	this.s.displayed = type;

	return true;
};


/**
 * Set the form into processing mode or take it out of processing mode. In
 * processing mode a processing indicator is shown and user interaction with the
 * form buttons is blocked
 *
 * @param {boolean} processing true if to go into processing mode and false if
 *   to come out of processing mode
 * @private
 */
Editor.prototype._processing = function ( processing )
{
	var procClass = this.classes.processing.active;

	$(['div.DTE', this.dom.wrapper]).toggleClass( procClass, processing );

	this.s.processing = processing;

	this._event( 'processing', [processing] );
};


/**
 * Submit a form to the server for processing. This is the private method that is used
 * by the 'submit' API method, which should always be called in preference to calling
 * this method directly.
 *
 * @param {function} [successCallback] Callback function that is executed once the
 *   form has been successfully submitted to the server and no errors occurred.
 * @param {function} [errorCallback] Callback function that is executed if the
 *   server reports an error due to the submission (this includes a JSON formatting
 *   error should the error return invalid JSON).
 * @param {function} [formatdata] Callback function that is passed in the data
 *   that will be submitted to the server, allowing pre-formatting of the data,
 *   removal of data or adding of extra fields.
 * @param {boolean} [hide=true] When the form is successfully submitted, by default
 *   the form display will be hidden - this option allows that to be overridden.
 * @private
 */
Editor.prototype._submit = function ( successCallback, errorCallback, formatdata, hide )
{
	var that = this;
	var i, iLen, eventRet, errorNodes;
	var changed = false, allData = {}, changedData = {};
	var setBuilder =  DataTable.ext.oApi._fnSetObjectDataFn;
	var dataSource = this.s.dataSource;
	var fields = this.s.fields;
	var editCount = this.s.editCount;
	var modifier = this.s.modifier;
	var editFields = this.s.editFields;
	var editData = this.s.editData;
	var opts = this.s.editOpts;
	var changedSubmit = opts.submit;
	var submitParamsLocal;
	
	if ( this._event( 'initSubmit', [this.s.action] ) === false ) {
		this._processing( false );
		return;
	}

	// After initSubmit to allow `mode()` to be used as a setter
	var action = this.s.action;
	var submitParams = {
		"action": action,
		"data": {}
	};

	// For backwards compatibility
	if ( this.s.dbTable ) {
		submitParams.table = this.s.dbTable;
	}

	// Gather the data that is to be submitted
	if ( action === "create" || action === "edit" ) {
		$.each( editFields, function ( idSrc, edit ) {
			var allRowData = {};
			var changedRowData = {};

			$.each( fields, function (name, field) {
				if ( edit.fields[ name ] ) {
					var value = field.multiGet( idSrc );
					var builder = setBuilder( name );
					var manyBuilder = $.isArray( value ) && name.indexOf('[]') !== -1 ?
						setBuilder( name.replace(/\[.*$/,'')+'-many-count' ) :
						null;

					builder( allRowData, value );

					// We need to tell the server-side if an array submission
					// actually has no elements so it knows if the array was
					// being submitted or not (since otherwise it doesn't know
					// if the array was empty, or just not being submitted)
					if ( manyBuilder ) {
						manyBuilder( allRowData, value.length );
					}

					// Build a changed object for if that is the selected data
					// type
					if ( action === 'edit' && (!editData[ name ] || ! field.compare( value, editData[ name ][ idSrc ]) ) ) {
						builder( changedRowData, value );
						changed = true;

						if ( manyBuilder ) {
							manyBuilder( changedRowData, value.length );
						}
					}
				}
			} );

			if ( ! $.isEmptyObject( allRowData ) ) {
				allData[ idSrc ] = allRowData;
			}

			if ( ! $.isEmptyObject( changedRowData ) ) {
				changedData[ idSrc ] = changedRowData;
			}
		} );

		// Decide what data to submit to the server for edit (create is all, always)
		if ( action === 'create' || changedSubmit === 'all' || (changedSubmit === 'allIfChanged' && changed) ) {
			submitParams.data = allData;
		}
		else if ( changedSubmit === 'changed' && changed ) {
			submitParams.data = changedData;
		}
		else {
			// Nothing to submit
			this.s.action = null;

			if ( opts.onComplete === 'close' && (hide === undefined || hide) ) {
				this._close( false );
			}
			else if ( typeof opts.onComplete === 'function' ) {
				opts.onComplete( this );
			}

			if ( successCallback ) {
				successCallback.call( this );
			}

			this._processing( false );
			this._event( 'submitComplete' );
			return;
		}
	}
	else if ( action === "remove" ) {
		$.each( editFields, function ( idSrc, edit ) {
			submitParams.data[ idSrc ] = edit.data;
		} );
	}

	this._legacyAjax( 'send', action, submitParams );

	// Local copy of the submit parameters, needed for the data lib prep since
	// the preSubmit can modify the format and we need to know what the format is
	submitParamsLocal = $.extend( true, {}, submitParams );

	// Allow the data to be submitted to the server to be preprocessed by callback
	// and event functions
	if ( formatdata ) {
		formatdata( submitParams );
	}
	if ( this._event( 'preSubmit', [submitParams, action] ) === false ) {
		this._processing( false );
		return;
	}

	// Submit to the server (or whatever method is defined in the settings)
	var submitWire = this.s.ajax || this.s.ajaxUrl ?
		this._ajax :
		this._submitTable;

	submitWire.call(
		this,
		submitParams,
		function (json, notGood, xhr) {
			that._submitSuccess(
				json, notGood, submitParams, submitParamsLocal, that.s.action,
				editCount, hide, successCallback, errorCallback, xhr
			);
		},
		function (xhr, err, thrown) {
			that._submitError( xhr, err, thrown, errorCallback, submitParams, that.s.action );
		},
		submitParams
	);
};


/**
 * Save submitted data without an Ajax request. This will write to a local
 * table only - not saving it permanently, but rather using the DataTable itself
 * as a data store.
 *
 * @param  {object} data Data to submit
 * @param  {function} success Success callback
 * @param  {function} error Error callback
 * @param  {object} submitParams Submitted data
 * @private
 */
Editor.prototype._submitTable = function ( data, success, error, submitParams )
{
	var that = this;
	var action = data.action;
	var out = { data: [] };
	var idGet = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
	var idSet = DataTable.ext.oApi._fnSetObjectDataFn( this.s.idSrc );

	// Nothing required for remove - create and edit get a copy of the data
	if ( action !== 'remove' ) {
		var originalData = this._dataSource( 'fields', this.modifier() );

		$.each( data.data, function ( key, vals ) {
			var toSave;

			// Get the original row's data, so we can modify it with new values.
			// This allows Editor to not need to submit all fields
			if ( action === 'edit' ) {
				var rowData = originalData[ key ].data;
				toSave = $.extend( true, {}, rowData, vals );
			}
			else {
				toSave = $.extend( true, {}, vals );
			}

			// If create and there isn't an id for the new row, create
			// one. An id could be creased by `preSubmit`
			if ( action === 'create' && idGet( toSave ) === undefined ) {
				idSet( toSave, +new Date() +''+ key );
			}
			else {
				idSet( toSave, key );
			}

			out.data.push( toSave );
		} );
	}

	success( out );
};


/**
 * Submit success callback function
 * @param  {object} json                Payload
 * @param  {bool} notGood               True if the returned status code was
 *   >=400 (i.e. processing failed). This is called `notGood` rather than
 *   `success` since the request was successfully processed, just not written to
 *   the db. It is also inverted from "good" to make it optional when overriding
 *   the `ajax` function.
 * @param  {object} submitParams        Submitted data
 * @param  {object} submitParamsLocal   Unmodified copy of submitted data
 *   (before it could be modified by the user)
 * @param  {string} action              CRUD action being taken
 * @param  {int} editCount              Protection against async errors
 * @param  {bool} hide                  Hide the form flag
 * @param  {function} successCallback   Success callback
 * @param  {function} errorCallback     Error callback
 * @private
 */
Editor.prototype._submitSuccess = function ( json, notGood, submitParams, submitParamsLocal, action, editCount, hide, successCallback, errorCallback, xhr )
{
	var that = this;
	var setData;
	var fields = this.s.fields;
	var opts = this.s.editOpts;
	var modifier = this.s.modifier;

	this._legacyAjax( 'receive', action, json );
	this._event( 'postSubmit', [json, submitParams, action, xhr] );

	if ( !json.error ) {
		json.error = "";
	}
	if ( !json.fieldErrors ) {
		json.fieldErrors = [];
	}

	if ( notGood || json.error || json.fieldErrors.length ) {
		// Global form error
		this.error( json.error );

		// Field specific errors
		$.each( json.fieldErrors, function (i, err) {
			var field = fields[ err.name ];

			field.error( err.status || "Error" );

			if ( i === 0 ) {
				if ( opts.onFieldError === 'focus' ) {
					// Scroll the display to the first error and focus
					$(that.dom.bodyContent, that.s.wrapper).animate( {
						"scrollTop": $(field.node()).position().top
					}, 500 );

					field.focus();
				}
				else if ( typeof opts.onFieldError === 'function' ) {
					opts.onFieldError( that, err );
				}
			}
		} );

		this._event( 'submitUnsuccessful', [json] );
		if ( errorCallback ) {
			errorCallback.call( that, json );
		}
	}
	else {
		// Create a data store that the data source can use, which is
		// unique to this action
		var store = {};

		if ( json.data && (action === "create"  || action === "edit") ) {
			this._dataSource( 'prep', action, modifier, submitParamsLocal, json, store );

			for ( var i=0 ; i<json.data.length ; i++ ) {
				setData = json.data[ i ];
				this._event( 'setData', [json, setData, action] ); // legacy

				if ( action === "create" ) {
					// New row was created to add it to the DT
					this._event( 'preCreate', [json, setData] );
					this._dataSource( 'create', fields, setData, store );
					this._event( ['create', 'postCreate'], [json, setData] );
				}
				else if ( action === "edit" ) {
					// Row was updated, so tell the DT
					this._event( 'preEdit', [json, setData] );
					this._dataSource( 'edit', modifier, fields, setData, store );
					this._event( ['edit', 'postEdit'], [json, setData] );
				}
			}

			this._dataSource( 'commit', action, modifier, json.data, store );
		}
		else if ( action === "remove" ) {
			this._dataSource( 'prep', action, modifier, submitParamsLocal, json, store );

			// Remove the rows given and then redraw the table
			this._event( 'preRemove', [json] );
			this._dataSource( 'remove', modifier, fields, store );
			this._event( ['remove', 'postRemove'], [json] );

			this._dataSource( 'commit', action, modifier, json.data, store );
		}

		// Submission complete
		if ( editCount === this.s.editCount ) {
			this.s.action = null;

			if ( opts.onComplete === 'close' && (hide === undefined || hide) ) {
				// If no data returned, then treat as not complete
				this._close( json.data ? true : false );
			}
			else if ( typeof opts.onComplete === 'function' ) {
				opts.onComplete( this );
			}
		}

		// All done - fire off the callbacks and events
		if ( successCallback ) {
			successCallback.call( that, json );
		}
		this._event( 'submitSuccess', [json, setData, action] );
	}

	this._processing( false );
	this._event( 'submitComplete', [json, setData, action] );
};


/**
 * Submit error callback function
 * @private
 */
Editor.prototype._submitError = function ( xhr, err, thrown, errorCallback, submitParams, action )
{
	this._event( 'postSubmit', [null, submitParams, action, xhr] );

	this.error( this.i18n.error.system );
	this._processing( false );

	if ( errorCallback ) {
		errorCallback.call( this, xhr, err, thrown );
	}

	this._event( ['submitError', 'submitComplete'], [xhr, err, thrown, submitParams] );
};


/**
 * Check to see if the form needs to be tidied before a new action can be performed.
 * This includes if the from is currently processing an old action and if it
 * is inline editing.
 *
 * @param {function} fn Callback function
 * @returns {boolean} `true` if was in inline mode, `false` otherwise
 * @private
 */
Editor.prototype._tidy = function ( fn )
{
	var that = this;
	var dt = this.s.table ?
		new $.fn.dataTable.Api( this.s.table ) :
		null;

	var ssp = false;
	if ( dt ) {
		ssp = dt.settings()[0].oFeatures.bServerSide;
	}

	if ( this.s.processing ) {
		// If currently processing, wait until the action is complete
		this.one( 'submitComplete', function () {
			// If server-side processing is being used in DataTables, first
			// check that we are still processing (might not be if nothing was
			// submitted) and then wait for the draw to finished
			if ( ssp ) {
				dt.one( 'draw', fn );
			}
			else {
				setTimeout( function () {
					fn();
				}, 10 );
			}
		} );

		return true;
	}
	else if ( this.display() === 'inline' || this.display() === 'bubble' ) {
		// If there is an inline edit box, it needs to be tidied
		this
			.one( 'close', function () {
				// On close if processing then we need to wait for the submit to
				// complete before running the callback as onBlur was set to
				// submit
				if ( ! that.s.processing ) {
					// IE needs a small timeout, otherwise it may not focus on a
					// field if one already has focus
					setTimeout( function () {
						fn();
					}, 10 );
				}
				else {
					// Need to wait for the submit to finish
					that.one( 'submitComplete', function ( e, json ) {
						// If SSP then need to wait for the draw
						if ( ssp && json ) {
							dt.one( 'draw', fn );
						}
						else {
							setTimeout( function () {
								fn();
							}, 10 );
						}
					} );
				}
			} )
			.blur();

		return true;
	}

	return false;
};

/**
 * Same as $.inArray but with weak type checking
 * @param {any} name Value to look for in the array
 * @param {array} arr Array to scan through
 * @returns {number} -1 if not found, index otherwise
 */
Editor.prototype._weakInArray = function ( name, arr )
{
	for ( var i=0, ien=arr.length ; i<ien ; i++ ) {
		if ( name == arr[i] ) {
			return i;
		}
	}

	return -1;
};

/*
 * Defaults
 */


// Dev node - although this file is held in the models directory (because it
// really is a model, it is assigned to Editor.defaults for easy
// and sensible access to set the defaults for Editor.

/**
 * Initialisation options that can be given to Editor at initialisation time.
 *  @namespace
 */
Editor.defaults = {
	/**
	 * jQuery selector that can be used to identify the table you wish to apply
	 * this editor instance to.
	 *
	 * In previous versions of Editor (1.2 and earlier), this parameter was
	 * called `table`. The name has been altered in 1.3+ to simplify the
	 * initialisation. This is a backwards compatible change - if you pass in
	 * a `table` option it will be used.
	 *  @type string
	 *  @default <i>Empty string</i>
	 *
	 *  @example
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": "php/index.php",
	 *        "table": "#example"
	 *      } );
	 *    } );
	 */
	"table": null,

	/**
	 * The URL, or collection of URLs when using a REST interface, which will accept 
	 * the data for the create, edit and remove functions. The target script / program
	 * must accept data in the format defined by Editor and return the expected JSON as
	 * required by Editor. When given as an object, the `create`, `edit` and `remove`
	 * properties should be defined, each being the URL to send the data to for that
	 * action. When used as an object, the string `_id_` will be replaced for the edit
	 * and remove actions, allowing a URL to be dynamically created for those actions.
	 *  @type string|object
	 *  @default <i>Empty string</i>
	 *  @deprecated This option has been deprecated in favour of the `ajax` option.
	 *    It can still be used, but it is recommended that you use the `ajax` option
	 *    which provides all of the abilities of this old option and more.
	 */
	"ajaxUrl": null,

	/**
	 * Fields to initialise the form with - see {@link Editor.models.field} for
	 * a full list of the options available to each field. Note that if fields are not 
	 * added to the form at initialisation time using this option, they can be added using
	 * the {@link Editor#add} API method.
	 *  @type array
	 *  @default []
	 *
	 *  @example
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": "php/index.php",
	 *        "table": "#example",
	 *        "fields": [ {
	 *            "label": "User name:",
	 *            "name": "username"
	 *          }
	 *          // More fields would typically be added here!
	 *        } ]
	 *      } );
	 *    } );
	 */
	"fields": [],

	/**
	 * The display controller for the form. The form itself is just a collection of
	 * DOM elements which require a display container. This display controller allows
	 * the visual appearance of the form to be significantly altered without major
	 * alterations to the Editor code. There are two display controllers built into
	 * Editor *lightbox* and *envelope*. The value of this property will
	 * be used to access the display controller defined in {@link Editor.display}
	 * for the given name. Additional display controllers can be added by adding objects
	 * to that object, through extending the displayController model:
	 * {@link Editor.models.displayController}.
	 *  @type string
	 *  @default lightbox
	 *
	 *  @example
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": "php/index.php",
	 *        "table": "#example",
	 *        "display": 'envelope'
	 *      } );
	 *    } );
	 */
	"display": 'lightbox',

	/**
	 * Control how the Ajax call to update data on the server.
	 *
	 * This option matches the `dt-init ajax` option in that is can be provided
	 * in one of three different ways:
	 *
	 * * string - As a string, the value given is used as the url to target
	 *   the Ajax request to, using the default Editor Ajax options. Note that
	 *   for backwards compatibility you can use the form "METHOD URL" - for
	 *   example: `"PUT api/users"`, although it is recommended you use the
	 *   object form described below.
	 * * object - As an object, the `ajax` property has two forms:
	 *   * Used to extend and override the default Ajax options that Editor
	 *     uses. This can be very useful for adding extra data for example, or
	 *     changing the HTTP request type.
	 *   * With `create`, `edit` and `remove` properties, Editor will use the
	 *     option for the action that it is taking, which can be useful for
	 *     REST style interfaces. The value of each property can be a string,
	 *     object or function, using exactly the same options as the main `ajax`
	 *     option. All three options must be defined if this form is to be used.
	 * * function - As a function this gives complete control over the method
	 *   used to update the server (if indeed a server is being used!). For
	 *   example, you could use a different data store such as localStorage,
	 *   Firebase or route the data through a web-socket.
	 *
	 *  @example
	 *    // As a string - all actions are submitted to this URI as POST requests
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": 'php/index.php',
	 *        "table": "#example"
	 *      } );
	 *    } );
	 *
	 *  @example
	 *    // As an object - using GET rather than POST
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": {
	 *          "type": 'GET',
	 *          "url": 'php/index.php
	 *        },
	 *        "table": "#example"
	 *      } );
	 *    } );
	 *
	 *  @example
	 *    // As an object - each action is submitted to a different URI as POST requests
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": {
	 *          "create": "/rest/user/create",
	 *          "edit":   "/rest/user/_id_/edit",
	 *          "remove": "/rest/user/_id_/delete"
	 *        },
	 *        "table": "#example"
	 *      } );
	 *    } );
	 *
	 *  @example
	 *    // As an object - with different HTTP methods for each action
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": {
	 *          "create": {
	 *          	type: 'POST',
	 *          	url:  '/rest/user/create'
	 *          },
	 *          "edit": {
	 *          	type: 'PUT',
	 *          	url:  '/rest/user/edit/_id_'
	 *          },
	 *          "remove": {
	 *          	type: 'DELETE',
	 *          	url:  '/rest/user/delete'
	 *          }
	 *        },
	 *        "table": "#example"
	 *      } );
	 *    } );
	 *
	 *    // As a function - Making a custom `$.ajax` call
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": "php/index.php",
	 *        "table": "#example",
	 *        "ajax": function ( method, url, data, successCallback, errorCallback ) {
	 *          $.ajax( {
	 *            "type": method,
	 *            "url":  url,
	 *            "data": data,
	 *            "dataType": "json",
	 *            "success": function (json) {
	 *              successCallback( json );
	 *            },
	 *            "error": function (xhr, error, thrown) {
	 *              errorCallback( xhr, error, thrown );
	 *            }
	 *          } );
	 *        }
	 *      } );
	 *    } );
	 */
	"ajax": null,

	/**
	 * JSON property from which to read / write the row's ID property (i.e. its
	 * unique column index that identifies the row to the database). By default
	 * Editor will use the `DT_RowId` property from the data source object
	 * (DataTable's magic property to set the DOM id for the row).
	 *
	 * If you want to read a parameter from the data source object instead of
	 * using `DT_RowId`, set this option to the property name to use.
	 *
	 * Like other data source options the `srcId` option can be given in dotted
	 * object notation to read nested objects.
	 *  @type null|string
	 *  @default DT_RowId
	 *
	 *  @example
	 *    // Using a data source such as:
	 *    // { "id":12, "browser":"Chrome", ... }
	 *    $(document).ready(function() {
	 *      var editor = new $.fn.Editor( {
	 *        "ajax": "php/index.php",
	 *        "table": "#example",
	 *        "idSrc": "id"
	 *      } );
	 *    } );
	 */
	"idSrc": 'DT_RowId',

	/**
	 * Events / callbacks - event handlers can be assigned as an individual function
	 * during initialisation using the parameters in this name space. The names, and
	 * the parameters passed to each callback match their event equivalent in the
	 * {@link Editor} object.
	 *  @namespace
	 *  @deprecated Since 1.3. Use the `on()` API method instead. Note that events
	 *    passed in do still operate as they did in 1.2- but are no longer
	 *    individually documented.
	 */
	"events": {},

	/**
	 * Internationalisation options for Editor. All client-side strings that the
	 * end user can see in the interface presented by Editor can be modified here.
	 *
	 * You may also wish to refer to the <a href="http://datatables.net/usage/i18n">
	 * DataTables internationalisation options</a> to provide a fully language 
	 * customised table interface.
	 *  @namespace
	 *
	 *  @example
	 *    // Set the 'create' button text. All other strings used are the
	 *    // default values.
	 *    var editor = new $.fn.Editor( {
	 *      "ajax": "data/source",
	 *      "table": "#example",
	 *      "i18n": {
	 *        "create": {
	 *          "button": "New user"
	 *        }
	 *      }
	 *    } );
	 *
	 *  @example
	 *    // Set the submit text for all three actions
	 *    var editor = new $.fn.Editor( {
	 *      "ajax": "data/source",
	 *      "table": "#example",
	 *      "i18n": {
	 *        "create": {
	 *          "submit": "Create new user"
	 *        },
	 *        "edit": {
	 *          "submit": "Update user"
	 *        },
	 *        "remove": {
	 *          "submit": "Remove user"
	 *        }
	 *      }
	 *    } );
	 */
	"i18n": {
		/**
		 * Strings used when working with the Editor 'create' action (creating new
		 * records).
		 *  @namespace
		 */
		"create": {
			/**
			 * TableTools button text
			 *  @type string
			 *  @default New
			 */
			"button": "New",

			/**
			 * Display container title (when showing the editor display)
			 *  @type string
			 *  @default Create new entry
			 */
			"title":  "Create new entry",

			/**
			 * Submit button text
			 *  @type string
			 *  @default Create
			 */
			"submit": "Create"
		},

		/**
		 * Strings used when working with the Editor 'edit' action (editing existing
		 * records).
		 *  @namespace
		 */
		"edit": {
			/**
			 * TableTools button text
			 *  @type string
			 *  @default Edit
			 */
			"button": "Edit",

			/**
			 * Display container title (when showing the editor display)
			 *  @type string
			 *  @default Edit entry
			 */
			"title":  "Edit entry",

			/**
			 * Submit button text
			 *  @type string
			 *  @default Update
			 */
			"submit": "Update"
		},

		/**
		 * Strings used when working with the Editor 'delete' action (deleting 
		 * existing records).
		 *  @namespace
		 */
		"remove": {
			/**
			 * TableTools button text
			 *  @type string
			 *  @default Delete
			 */
			"button": "Delete",

			/**
			 * Display container title (when showing the editor display)
			 *  @type string
			 *  @default Delete
			 */
			"title":  "Delete",

			/**
			 * Submit button text
			 *  @type string
			 *  @default Delete
			 */
			"submit": "Delete",

			/**
			 * Deletion confirmation message.
			 *
			 * As Editor has the ability to delete either a single or multiple rows
			 * at a time, this option can be given as either a string (which will be
			 * used regardless of how many records are selected) or as an object 
			 * where the property "_" will be used (with %d substituted for the number
			 * of records to be deleted) as the delete message, unless there is a
			 * key with the number of records to be deleted. This allows Editor
			 * to consider the different pluralisation characteristics of different
			 * languages.
			 *  @type object|string
			 *  @default Are you sure you wish to delete %d rows?
			 *
			 *  @example
			 *    // String - no plural consideration
			 *    var editor = new $.fn.Editor( {
			 *      "ajax": "data/source",
			 *      "table": "#example",
			 *      "i18n": {
			 *        "remove": {
			 *          "confirm": "Are you sure you wish to delete %d record(s)?"
			 *        }
			 *      }
			 *    } );
			 *
			 *  @example
			 *    // Basic 1 (singular) or _ (plural)
			 *    var editor = new $.fn.Editor( {
			 *      "ajax": "data/source",
			 *      "table": "#example",
			 *      "i18n": {
			 *        "remove": {
			 *          "confirm": {
			 *            "_": "Confirm deletion of %d records.",
			 *            "1": "Confirm deletion of record."
			 *        }
			 *      }
			 *    } );
			 *
			 *  @example
			 *    // Singular, dual and plural
			 *    var editor = new $.fn.Editor( {
			 *      "ajax": "data/source",
			 *      "table": "#example",
			 *      "i18n": {
			 *        "remove": {
			 *          "confirm": {
			 *            "_": "Confirm deletion of %d records.",
			 *            "1": "Confirm deletion of record.",
			 *            "2": "Confirm deletion of both record."
			 *        }
			 *      }
			 *    } );
			 *        
			 */
			"confirm": {
				"_": "Are you sure you wish to delete %d rows?",
				"1": "Are you sure you wish to delete 1 row?"
			}
		},

		/**
		 * Strings used for error conditions.
		 *  @namespace
		 */
		"error": {
			/**
			 * Generic server error message
			 *  @type string
			 *  @default A system error has occurred (<a target=\"_blank\" href=\"//datatables.net/tn/12\">More information</a>)
			 */
			"system": "A system error has occurred (<a target=\"_blank\" href=\"//datatables.net/tn/12\">More information</a>)."
		},

		/**
		 * Strings used for multi-value editing
		 *  @namespace
		 */
		multi: {
			/**
			 * Shown in place of the field value when a field has multiple values
			 */
			title: "Multiple values",

			/**
			 * Shown below the multi title text, although only the first
			 * instance of this text is shown in the form to reduce redundancy
			 */
			info: "The selected items contain different values for this input. To edit and set all items for this input to the same value, click or tap here, otherwise they will retain their individual values.",

			/**
			 * Shown below the field input when group editing a value to allow
			 * the user to return to the original multiple values
			 */
			restore: "Undo changes",


			/**
			 * Disabled for multi-row editing
			 */
			noMulti: "This input can be edited individually, but not part of a group."
		},

		"datetime": {
			previous: 'Previous',
			next:     'Next',
			months:   [ 'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December' ],
			weekdays: [ 'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat' ],
			amPm:     [ 'am', 'pm' ],
			unknown:  '-'
		}
	},

	formOptions: {
		bubble: $.extend( {}, Editor.models.formOptions, {
			title: false,
			message: false,
			buttons: '_basic',
			submit: 'changed'
		} ),

		inline: $.extend( {}, Editor.models.formOptions, {
			buttons: false,
			submit: 'changed'
		} ),

		main: $.extend( {}, Editor.models.formOptions )
	},

	/**
	 * Submit data to the server in the 1.4- data format (`true`) or in the 1.5+
	 * data format (`false` - default).
	 *
	 * @type Boolean
	 */
	legacyAjax: false
};


/*
 * Extensions
 */

(function(){


var __dataSources = Editor.dataSources = {};


/* -  -  -  -  -  -  -  -  -  -
 * DataTables editor interface
 */

var __dtIsSsp = function ( dt, editor ) {
	// If the draw type is `none`, then we still need to use the DT API to
	// update the display with the new data
	return dt.settings()[0].oFeatures.bServerSide &&
		editor.s.editOpts.drawType !== 'none';
};

var __dtApi = function ( table ) {
	return $(table).DataTable();
};

var __dtHighlight = function ( node ) {
	// Highlight a row using CSS transitions. The timeouts need to match the
	// transition duration from the CSS
	node = $(node);

	setTimeout( function () {
		node.addClass( 'highlight' );

		setTimeout( function () {
			node
				.addClass( 'noHighlight' )
				.removeClass( 'highlight' );

			setTimeout( function () {
				node.removeClass( 'noHighlight' );
			}, 550 );
		}, 500 );
	}, 20 );
};

var __dtRowSelector = function ( out, dt, identifier, fields, idFn )
{
	dt.rows( identifier ).indexes().each( function ( idx ) {
		var row = dt.row( idx );
		var data = row.data();
		var idSrc = idFn( data );

		if ( idSrc === undefined ) {
			Editor.error( 'Unable to find row identifier', 14 );
		}

		out[ idSrc ] = {
			idSrc:  idSrc,
			data:   data,
			node:   row.node(),
			fields: fields,
			type:   'row'
		};
	} );
};

var __dtFieldsFromIdx = function ( dt, fields, idx )
{
	var field;
	var col = dt.settings()[0].aoColumns[ idx ];
	var dataSrc = col.editField !== undefined ?
		col.editField :
		col.mData;
	var resolvedFields = {};
	var run = function ( field, dataSrc ) {
		if ( field.name() === dataSrc ) {
			resolvedFields[ field.name() ] = field;
		}
	};

	$.each( fields, function ( name, fieldInst ) {
		if ( $.isArray( dataSrc ) ) {
			for ( var i=0 ; i<dataSrc.length ; i++ ) {
				run( fieldInst, dataSrc[i] );
			}
		}
		else {
			run( fieldInst, dataSrc );
		}
	} );

	if ( $.isEmptyObject( resolvedFields ) ) {
		Editor.error('Unable to automatically determine field from source. Please specify the field name.', 11);
	}

	return resolvedFields;
};

var __dtCellSelector = function ( out, dt, identifier, allFields, idFn, forceFields )
{
	dt.cells( identifier ).indexes().each( function ( idx ) {
		var cell = dt.cell( idx );
		var row = dt.row( idx.row );
		var data = row.data();
		var idSrc = idFn( data );
		var fields = forceFields || __dtFieldsFromIdx( dt, allFields, idx.column );
		var isNode = (typeof identifier === 'object' && identifier.nodeName) || identifier instanceof $;

		// Use the row selector to get the row information
		__dtRowSelector(out, dt, idx.row, allFields, idFn);

		out[ idSrc ].attach = isNode ?
			[ $(identifier).get(0) ] :
			[ cell.node() ];
		out[ idSrc ].displayFields = fields; // all fields are edited, but only
		                                     // these are actually displayed
	} );
};

var __dtColumnSelector = function ( out, dt, identifier, fields, idFn )
{
	dt.cells( null, identifier ).indexes().each( function ( idx ) {
		__dtCellSelector( out, dt, idx, fields, idFn );
	} );
};

var __dtjqId = function ( id ) {
	return typeof id === 'string' ?
		'#'+id.replace( /(:|\.|\[|\]|,)/g, '\\$1' ) :
		'#'+id;
};



__dataSources.dataTable = {
	individual: function ( identifier, fieldNames ) {
		var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
		var dt = __dtApi( this.s.table );
		var fields = this.s.fields;
		var out = {};
		var forceFields;
		var responsiveNode;

		if ( fieldNames ) {
			if ( ! $.isArray( fieldNames ) ) {
				fieldNames = [ fieldNames ];
			}

			forceFields = {};

			$.each( fieldNames, function ( i, name ) {
				forceFields[ name ] = fields[ name ];
			} );
		}

		__dtCellSelector( out, dt, identifier, fields, idFn, forceFields );

		return out;
	},

	// get idSrc, fields to edit, data and node for each item
	fields: function ( identifier )
	{
		var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
		var dt = __dtApi( this.s.table );
		var fields = this.s.fields;
		var out = {};

		if ( $.isPlainObject( identifier ) && ( identifier.rows !== undefined || identifier.columns !== undefined || identifier.cells !== undefined ) ) {
			// Multi-item type selector
			if ( identifier.rows !== undefined ) {
				__dtRowSelector( out, dt, identifier.rows, fields, idFn );
			}

			if ( identifier.columns !== undefined ) {
				__dtColumnSelector( out, dt, identifier.columns, fields, idFn );
			}
			
			if ( identifier.cells !== undefined ) {
				__dtCellSelector( out, dt, identifier.cells, fields, idFn );
			}
		}
		else {
			// Just a rows selector
			__dtRowSelector( out, dt, identifier, fields, idFn );
		}

		return out;
	},

	create: function ( fields, data ) {
		var dt = __dtApi( this.s.table );

		if ( ! __dtIsSsp( dt, this ) ) {
			var row = dt.row.add( data );
			__dtHighlight( row.node() );
		}
	},

	edit: function ( identifier, fields, data, store ) {
		var dt = __dtApi( this.s.table );

		// No point in doing anything when server-side processing - the commit
		// will redraw the table
		if ( ! __dtIsSsp( dt, this ) || this.s.editOpts.drawType === 'none' ) {
			// The identifier can select one or more rows, but the data will
			// refer to just a single row. We need to determine which row from
			// the set is the one to operator on.
			var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
			var rowId = idFn( data );
			var row;

			// Find the row to edit - attempt to do an id look up first for speed
			try {
				row = dt.row( __dtjqId(rowId) );
			}
			catch (e) {
				row = dt;
			}

			// If not found, then we need to do it the slow way
			if ( ! row.any() ) {
				row = dt.row( function ( rowIdx, rowData, rowNode ) {
					return rowId == idFn( rowData );
				} );
			}

			if ( row.any() ) {
				row.data( data );

				// Remove the item from the list of indexes now that is has been
				// updated
				var idx = $.inArray( rowId, store.rowIds );
				store.rowIds.splice( idx, 1 );
			}
			else {
				// If not found, then its a new row (change in pkey possibly)
				row = dt.row.add( data );
			}

			__dtHighlight( row.node() );
		}
	},

	remove: function ( identifier, fields, store ) {
		// No confirmation from the server 
		var dt = __dtApi( this.s.table );
		var cancelled = store.cancelled;

		if ( cancelled.length === 0 ) {
			// No rows were cancelled on the server-side, remove them all
			dt.rows( identifier ).remove();
		}
		else {
			// One or more rows were cancelled, so we need to identify them
			// and not remove those rows
			var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
			var indexes = [];

			dt.rows( identifier ).every( function () {
				var id = idFn( this.data() );

				if ( $.inArray( id, cancelled ) === -1 ) {
					// Don't use `remove` here - it messes up the indexes
					indexes.push( this.index() );
				}
			} );

			dt.rows( indexes ).remove();
		}
	},

	prep: function ( action, identifier, submit, json, store ) {
		// On edit we store the ids of the rows that are being edited
		if ( action === 'edit' ) {
			var cancelled = json.cancelled || [];

			store.rowIds = $.map( submit.data, function ( val, key ) {
				return ! $.isEmptyObject( submit.data[ key ] ) && // was submitted
					$.inArray( key, cancelled ) === -1 ? // was not cancelled on the server-side
						key :
						undefined;
			} );
		}
		else if ( action === 'remove' ) {
			store.cancelled = json.cancelled || [];
		}
	},

	commit: function ( action, identifier, data, store ) {
		// Updates complete - redraw
		var dt = __dtApi( this.s.table );

		// On edit, if there are any rows left in the `store.rowIds`, then they
		// were not returned by the server and should be removed (they might not
		// meet filtering requirements any more for example)
		if ( action === 'edit' && store.rowIds.length ) {
			var ids = store.rowIds;
			var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
			var row;
			var compare = function ( id ) {
				return function ( rowIdx, rowData, rowNode ) {
					return id == idFn( rowData );
				};
			};

			for ( var i=0, ien=ids.length ; i<ien ; i++ ) {
				// Find the row to edit - attempt to do an id look up first for speed
				row = dt.row( __dtjqId(ids[i]) );

				// If not found, then we need to do it the slow way
				if ( ! row.any() ) {
					row = dt.row( compare( ids[i] ) );
				}

				if ( row.any() ) {
					row.remove();
				}
			}
		}

		var drawType = this.s.editOpts.drawType;
		if ( drawType !== 'none' ) {
			dt.draw( drawType );
		}
	}
};



/* -  -  -  -  -  -  -  -
 * HTML editor interface
 */

function __html_el ( identifier, name ) {
	var context = identifier === 'keyless' ?
		document :
		$('[data-editor-id="'+identifier+'"]');

	return $('[data-editor-field="'+name+'"]', context);
}

function __html_els ( identifier, names ) {
	var out = $();

	for ( var i=0, ien=names.length ; i<ien ; i++ ) {
		out = out.add( __html_el( identifier, names[i] ) );
	}

	return out;
}

function __html_get( identifier, dataSrc ) {
	var el = __html_el( identifier, dataSrc );

	return el.filter('[data-editor-value]').length ?
		el.attr( 'data-editor-value' ) :
		el.html();
}

function __html_set( identifier, fields, data ) {
	$.each( fields, function ( name, field ) {
		var val = field.valFromData( data );

		if ( val !== undefined ) {
			var el = __html_el( identifier, field.dataSrc() );

			if ( el.filter('[data-editor-value]').length ) {
				el.attr( 'data-editor-value', val );
			}
			else {
				el.each( function () {
					// This is very frustrating, but in IE if you just write directly
					// to innerHTML, and elements that are overwritten are GC'ed,
					// even if there is a reference to them elsewhere
					while ( this.childNodes.length ) {
						this.removeChild( this.firstChild );
					}
				} )
				.html( val );
			}
		}
	} );
}



__dataSources.html = {
	initField: function ( cfg ) {
		// This is before the field has been initialised so can't use it API
		var label = $('[data-editor-label="'+(cfg.data || cfg.name)+'"]');
		if ( ! cfg.label && label.length ) {
			cfg.label = label.html();
		}
	},

	individual: function ( identifier, fieldNames ) {
		var attachEl;

		// Auto detection of the field name and id
		if ( identifier instanceof $ || identifier.nodeName ) {
			attachEl = identifier;

			if ( ! fieldNames ) {
				fieldNames = [ $( identifier ).attr('data-editor-field') ];
			}

			var back = $.fn.addBack ? 'addBack' : 'andSelf';
			identifier = $( identifier ).parents('[data-editor-id]')[ back ]().data('editor-id');
		}

		// no id given and none found
		if ( ! identifier ) {
			identifier = 'keyless';
		}

		// no field name - cannot continue
		if ( fieldNames && ! $.isArray( fieldNames ) ) {
			fieldNames = [ fieldNames ];
		}

		if ( ! fieldNames || fieldNames.length === 0 ) {
			throw 'Cannot automatically determine field name from data source';
		}

		var out = __dataSources.html.fields.call( this, identifier );
		var fields = this.s.fields;
		var forceFields = {};

		$.each( fieldNames, function ( i, name ) {
			forceFields[ name ] = fields[ name ];
		} );

		$.each( out, function ( id, set ) {
			set.type = 'cell';
			set.attach = attachEl ?
				$(attachEl) :
				__html_els( identifier, fieldNames ).toArray();
			set.fields = fields;
			set.displayFields = forceFields;
		} );

		return out;
	},

	// get idSrc, fields to edit, data and node for each item
	fields: function ( identifier )
	{
		var out = {};
		var data = {};
		var fields = this.s.fields;

		if ( ! identifier ) {
			identifier = 'keyless';
		}

		$.each( fields, function ( name, field ) {
			var val = __html_get( identifier, field.dataSrc() );

			// If no HTML element is present, jQuery returns null. We want undefined
			field.valToData( data, val === null ? undefined : val );
		} );

		out[ identifier ] = {
			idSrc: identifier,
			data: data,
			node: document,
			fields: fields,
			type: 'row'
		};

		return out;
	},

	create: function ( fields, data ) {
		// If there is an element with the id that has been created, then use it
		// to assign the values
		if ( data ) {
			var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
			var id = idFn( data );

			if ( $('[data-editor-id="'+id+'"]').length ) {
				__html_set( id, fields, data );
			}
		}
	},

	edit: function ( identifier, fields, data ) {
		// Get the ids from the returned data or `keyless` if not found
		var idFn = DataTable.ext.oApi._fnGetObjectDataFn( this.s.idSrc );
		var id = idFn( data ) || 'keyless';

		__html_set( id, fields, data );
	},

	remove: function ( identifier, fields ) {
		// If there is an element with an ID property matching the identifier,
		// remove it
		$('[data-editor-id="'+identifier+'"]').remove();
	}
};


}());



/**
 * Class names that are used by Editor for its various display components.
 * A copy of this object is taken when an Editor instance is initialised, thus
 * allowing different classes to be used in different instances if required.
 * Class name changes can be useful for easy integration with CSS frameworks,
 * for example Twitter Bootstrap.
 *  @namespace
 */
Editor.classes = {
	/**
	 * Applied to the base DIV element that contains all other Editor elements
	 */
	"wrapper": "DTE",

	/**
	 * Processing classes
	 *  @namespace
	 */
	"processing": {
		/**
		 * Processing indicator element
		 */
		"indicator": "DTE_Processing_Indicator",

		/**
		 * Added to the base element ("wrapper") when the form is "processing"
		 */
		"active": "processing"
	},

	/**
	 * Display header classes
	 *  @namespace
	 */
	"header": {
		/**
		 * Container for the header elements
		 */
		"wrapper": "DTE_Header",

		/**
		 * Liner for the header content
		 */
		"content": "DTE_Header_Content"
	},

	/**
	 * Display body classes
	 *  @namespace
	 */
	"body": {
		/**
		 * Container for the body elements
		 */
		"wrapper": "DTE_Body",

		/**
		 * Liner for the body content
		 */
		"content": "DTE_Body_Content"
	},

	/**
	 * Display footer classes
	 *  @namespace
	 */
	"footer": {
		/**
		 * Container for the footer elements
		 */
		"wrapper": "DTE_Footer",
		
		/**
		 * Liner for the footer content
		 */
		"content": "DTE_Footer_Content"
	},

	/**
	 * Form classes
	 *  @namespace
	 */
	"form": {
		/**
		 * Container for the form elements
		 */
		"wrapper": "DTE_Form",

		/**
		 * Liner for the form content
		 */
		"content": "DTE_Form_Content",

		/**
		 * Applied to the <form> tag
		 */
		"tag":     "",

		/**
		 * Global form information
		 */
		"info":    "DTE_Form_Info",

		/**
		 * Global error imformation
		 */
		"error":   "DTE_Form_Error",

		/**
		 * Buttons container
		 */
		"buttons": "DTE_Form_Buttons",

		/**
		 * Buttons container
		 */
		"button": "btn"
	},

	/**
	 * Field classes
	 *  @namespace
	 */
	"field": {
		/**
		 * Container for each field
		 */
		"wrapper":     "DTE_Field",

		/**
		 * Class prefix for the field type - field type is added to the end allowing
		 * styling based on field type.
		 */
		"typePrefix":  "DTE_Field_Type_",

		/**
		 * Class prefix for the field name - field name is added to the end allowing
		 * styling based on field name.
		 */
		"namePrefix":  "DTE_Field_Name_",

		/**
		 * Field label
		 */
		"label":       "DTE_Label",

		/**
		 * Field input container
		 */
		"input":       "DTE_Field_Input",

		/**
		 * Input elements wrapper
		 */
		"inputControl": "DTE_Field_InputControl",

		/**
		 * Field error state (added to the field.wrapper element when in error state
		 */
		"error":       "DTE_Field_StateError",

		/**
		 * Label information text
		 */
		"msg-label":   "DTE_Label_Info",

		/**
		 * Error information text
		 */
		"msg-error":   "DTE_Field_Error",

		/**
		 * Live messaging (API) information text
		 */
		"msg-message": "DTE_Field_Message",

		/**
		 * General information text
		 */
		"msg-info":    "DTE_Field_Info",

		/**
		 * Multi-value information display wrapper
		 */
		"multiValue":  "multi-value",

		/**
		 * Multi-value information descriptive text
		 */
		"multiInfo":  "multi-info",

		/**
		 * Multi-value information display
		 */
		"multiRestore":  "multi-restore",

		/**
		 * Multi-value not editable (field.multiEditable)
		 */
		"multiNoEdit": "multi-noEdit",

		/**
		 * Field is disabled
		 */
		"disabled": "disabled"
	},

	/**
	 * Action classes - these are added to the Editor base element ("wrapper")
	 * and allows styling based on the type of form view that is being employed.
	 *  @namespace
	 */
	"actions": {
		/**
		 * Editor is in 'create' state
		 */
		"create": "DTE_Action_Create",

		/**
		 * Editor is in 'edit' state
		 */
		"edit":   "DTE_Action_Edit",

		/**
		 * Editor is in 'remove' state
		 */
		"remove": "DTE_Action_Remove"
	},

	/**
	 * Inline editing classes - these are used to display the inline editor
	 *  @namespace
	 */
	"inline": {
		"wrapper": "DTE DTE_Inline",

		"liner": "DTE_Inline_Field",

		"buttons": "DTE_Inline_Buttons"
	},

	/**
	 * Bubble editing classes - these are used to display the bubble editor
	 *  @namespace
	 */
	"bubble": {
		/**
		 * Bubble container element
		 */
		"wrapper": "DTE DTE_Bubble",

		/**
		 * Bubble content liner
		 */
		"liner": "DTE_Bubble_Liner",

		/**
		 * Bubble table display wrapper, so the buttons and form can be shown
		 * as table cells (via css)
		 */
		"table": "DTE_Bubble_Table",

		/**
		 * Close button
		 */
		"close": "icon close",

		/**
		 * Pointer shown which node is being edited
		 */
		"pointer": "DTE_Bubble_Triangle",

		/**
		 * Fixed background
		 */
		"bg": "DTE_Bubble_Background"
	}
};


/*
 * Add helpful buttons to make life easier
 *
 * Note that the values that require a string to make any sense (the button text
 * for example) are set by Editor when Editor is initialised through the i18n
 * options.
 */
(function () {

if ( DataTable.TableTools ) {
	var ttButtons = DataTable.TableTools.BUTTONS;
	var ttButtonBase = {
		sButtonText: null,
		editor:      null,
		formTitle:   null
	};

	ttButtons.editor_create = $.extend( true, ttButtons.text, ttButtonBase, {
		formButtons: [ {
			label: null,
			fn: function (e) { this.submit(); }
		} ],

		fnClick: function( button, config ) {
			var editor = config.editor;
			var i18nCreate = editor.i18n.create;
			var buttons = config.formButtons;

			if ( ! buttons[0].label ) {
				buttons[0].label = i18nCreate.submit;
			}

			editor.create( {
				title: i18nCreate.title,
				buttons: buttons
			} );
		}
	} );


	ttButtons.editor_edit = $.extend( true, ttButtons.select_single, ttButtonBase, {
		formButtons: [ {
			label: null,
			fn: function (e) { this.submit(); }
		} ],

		fnClick: function( button, config ) {
			var selected = this.fnGetSelectedIndexes();
			if ( selected.length !== 1 ) {
				return;
			}

			var editor = config.editor;
			var i18nEdit = editor.i18n.edit;
			var buttons = config.formButtons;

			if ( ! buttons[0].label ) {
				buttons[0].label = i18nEdit.submit;
			}

			editor.edit( selected[0], {
				title: i18nEdit.title,
				buttons: buttons
			} );
		}
	} );


	ttButtons.editor_remove = $.extend( true, ttButtons.select, ttButtonBase, {
		question: null,

		formButtons: [
			{
				label: null,
				fn: function (e) {
					// Executed in the Form instance's scope
					var that = this;
					this.submit( function ( json ) {
						var tt = $.fn.dataTable.TableTools.fnGetInstance(
							$(that.s.table).DataTable().table().node()
						);
						tt.fnSelectNone();
					} );
				}
			}
		],

		fnClick: function( button, config ) {
			var rows = this.fnGetSelectedIndexes();
			if ( rows.length === 0 ) {
				return;
			}

			var editor = config.editor;
			var i18nRemove = editor.i18n.remove;
			var buttons = config.formButtons;
			var question = typeof i18nRemove.confirm === 'string' ?
				i18nRemove.confirm :
				i18nRemove.confirm[rows.length] ?
					i18nRemove.confirm[rows.length] : i18nRemove.confirm._;

			if ( ! buttons[0].label ) {
				buttons[0].label = i18nRemove.submit;
			}

			editor.remove( rows, {
				message: question.replace( /%d/g, rows.length ),
				title: i18nRemove.title,
				buttons: buttons
			} );
		}
	} );
}

var _buttons = DataTable.ext.buttons;


$.extend( _buttons, {
	create: {
		text: function ( dt, node, config ) {
			return dt.i18n( 'buttons.create', config.editor.i18n.create.button );
		},
		className: 'buttons-create',
		editor: null,
		formButtons: {
			text: function ( editor ) {
				return editor.i18n.create.submit;
			},
			action: function (e) {
				this.submit();
			}
		},
		formMessage: null,
		formTitle: null,
		action: function( e, dt, node, config ) {
			var editor = config.editor;
			var buttons = config.formButtons;

			editor.create( {
				buttons: config.formButtons,
				message: config.formMessage,
				title:   config.formTitle || editor.i18n.create.title
			} );
		}
	},

	edit: {
		extend: 'selected',
		text: function ( dt, node, config ) {
			return dt.i18n( 'buttons.edit', config.editor.i18n.edit.button );
		},
		className: 'buttons-edit',
		editor: null,
		formButtons: {
			text: function ( editor ) {
				return editor.i18n.edit.submit;
			},
			action: function (e) {
				this.submit();
			}
		},
		formMessage: null,
		formTitle: null,
		action: function( e, dt, node, config ) {
			var editor = config.editor;
			var rows = dt.rows( { selected: true } ).indexes();
			var columns = dt.columns( { selected: true } ).indexes();
			var cells = dt.cells( { selected: true } ).indexes();

			var items = columns.length || cells.length ?
				{
					rows: rows,
					columns: columns,
					cells: cells
				} :
				rows;

			editor.edit( items, {
				message: config.formMessage,
				buttons: config.formButtons,
				title:   config.formTitle || editor.i18n.edit.title
			} );
		}
	},

	remove: {
		extend: 'selected',
		text: function ( dt, node, config ) {
			return dt.i18n( 'buttons.remove', config.editor.i18n.remove.button );
		},
		className: 'buttons-remove',
		editor: null,
		formButtons: {
			text: function ( editor ) {
				return editor.i18n.remove.submit;
			},
			action: function (e) {
				this.submit();
			}
		},
		formMessage: function ( editor, dt ) {
			var rows = dt.rows( { selected: true } ).indexes();
			var i18n = editor.i18n.remove;
			var question = typeof i18n.confirm === 'string' ?
				i18n.confirm :
				i18n.confirm[rows.length] ?
					i18n.confirm[rows.length] : i18n.confirm._;

			return question.replace( /%d/g, rows.length );
		},
		formTitle: null,
		action: function( e, dt, node, config ) {
			var editor = config.editor;

			editor.remove( dt.rows( { selected: true } ).indexes(), {
				buttons: config.formButtons,
				message: config.formMessage,
				title:   config.formTitle || editor.i18n.remove.title
			} );
		}
	}
} );

// Reuse the standard edit and remove buttons for their singular equivalent,
// but set it to extend the single selected button only
_buttons.editSingle = $.extend( {}, _buttons.edit );
_buttons.editSingle.extend = 'selectedSingle';

_buttons.removeSingle = $.extend( {}, _buttons.remove );
_buttons.removeSingle.extend = 'selectedSingle';

}());

/**
 * Field types array - this can be used to add field types or modify the pre-
 * defined options. See the online Editor documentation for information about
 * the built in field types.
 *
 * Note that we use a DataTables ext object to allow plug-ins to be loaded
 * before Editor itself. This is useful for the online DataTables download
 * builder.
 *
 *  @namespace
 */
Editor.fieldTypes = {};
/*
 * This file provides a DateTime GUI picker (calendar and time input). Only the
 * format YYYY-MM-DD is supported without additional software, but the end user
 * experience can be greatly enhanced by including the momentjs library which
 * provides date / time parsing and formatting options.
 *
 * This functionality is required because the HTML5 date and datetime input
 * types are not widely supported in desktop browsers.
 *
 * Constructed by using:
 *
 *     new Editor.DateTime( input, opts )
 *
 * where `input` is the HTML input element to use and `opts` is an object of
 * options based on the `Editor.DateTime.defaults` object.
 */

Editor.DateTime = function ( input, opts ) {
	this.c = $.extend( true, {}, Editor.DateTime.defaults, opts );
	var classPrefix = this.c.classPrefix;
	var i18n = this.c.i18n;

	// Only IS8601 dates are supported without moment
	if ( ! window.moment && this.c.format !== 'YYYY-MM-DD' ) {
		throw "Editor datetime: Without momentjs only the format 'YYYY-MM-DD' can be used";
	}

	var timeBlock = function ( type ) {
		return '<div class="'+classPrefix+'-timeblock">'+
				'<div class="'+classPrefix+'-iconUp">'+
					'<button>'+i18n.previous+'</button>'+
				'</div>'+
				'<div class="'+classPrefix+'-label">'+
					'<span/>'+
					'<select class="'+classPrefix+'-'+type+'"/>'+
				'</div>'+
				'<div class="'+classPrefix+'-iconDown">'+
					'<button>'+i18n.next+'</button>'+
				'</div>'+
			'</div>';
	};

	var gap = function () {
		return '<span>:</span>';
	};

	// DOM structure
	var structure = $(
		'<div class="'+classPrefix+'">'+
			'<div class="'+classPrefix+'-date">'+
				'<div class="'+classPrefix+'-title">'+
					'<div class="'+classPrefix+'-iconLeft">'+
						'<button>'+i18n.previous+'</button>'+
					'</div>'+
					'<div class="'+classPrefix+'-iconRight">'+
						'<button>'+i18n.next+'</button>'+
					'</div>'+
					'<div class="'+classPrefix+'-label">'+
						'<span/>'+
						'<select class="'+classPrefix+'-month"/>'+
					'</div>'+
					'<div class="'+classPrefix+'-label">'+
						'<span/>'+
						'<select class="'+classPrefix+'-year"/>'+
					'</div>'+
				'</div>'+
				'<div class="'+classPrefix+'-calendar"/>'+
			'</div>'+
			'<div class="'+classPrefix+'-time">'+
				timeBlock( 'hours' )+
				gap()+
				timeBlock( 'minutes' )+
				gap()+
				timeBlock( 'seconds' )+
				timeBlock( 'ampm' )+
			'</div>'+
			'<div class="'+classPrefix+'-error"/>'+
		'</div>'
	);

	this.dom = {
		container: structure,
		date:      structure.find( '.'+classPrefix+'-date' ),
		title:     structure.find( '.'+classPrefix+'-title' ),
		calendar:  structure.find( '.'+classPrefix+'-calendar' ),
		time:      structure.find( '.'+classPrefix+'-time' ),
		error:     structure.find( '.'+classPrefix+'-error' ),
		input:     $(input)
	};

	this.s = {
		/** @type {Date} Date value that the picker has currently selected */
		d: null,

		/** @type {Date} Date of the calendar - might not match the value */
		display: null,

		/** @type {String} Unique namespace string for this instance */
		namespace: 'editor-dateime-'+(Editor.DateTime._instance++),

		/** @type {Object} Parts of the picker that should be shown */
		parts: {
			date:    this.c.format.match( /[YMD]|L(?!T)|l/ ) !== null,
			time:    this.c.format.match( /[Hhm]|LT|LTS/ ) !== null,
			seconds: this.c.format.indexOf( 's' )   !== -1,
			hours12: this.c.format.match( /[haA]/ ) !== null
		}
	};

	this.dom.container
		.append( this.dom.date )
		.append( this.dom.time )
		.append( this.dom.error );

	this.dom.date
		.append( this.dom.title )
		.append( this.dom.calendar );

	this._constructor();
};

$.extend( Editor.DateTime.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public
	 */
	
	/**
	 * Destroy the control
	 */
	destroy: function () {
		this._hide();
		this.dom.container.off().empty();
		this.dom.input.off('.editor-datetime');
	},

	errorMsg: function ( msg ) {
		var error = this.dom.error;

		if ( msg ) {
			error.html( msg );
		}
		else {
			error.empty();
		}
	},

	hide: function () {
		this._hide();
	},

	max: function ( date ) {
		this.c.maxDate = date;

		this._optionsTitle();
		this._setCalander();
	},

	min: function ( date ) {
		this.c.minDate = date;

		this._optionsTitle();
		this._setCalander();
	},

	/**
	 * Check if an element belongs to this control
	 *
	 * @param  {node} node Element to check
	 * @return {boolean}   true if owned by this control, false otherwise
	 */
	owns: function ( node ) {
		return $(node).parents().filter( this.dom.container ).length > 0;
	},

	/**
	 * Get / set the value
	 *
	 * @param  {string|Date} set   Value to set
	 * @param  {boolean} [write=true] Flag to indicate if the formatted value
	 *   should be written into the input element
	 */
	val: function ( set, write ) {
		if ( set === undefined ) {
			return this.s.d;
		}

		if ( set instanceof Date ) {
			this.s.d = this._dateToUtc( set );
		}
		else if ( set === null || set === '' ) {
			this.s.d = null;
		}
		else if ( typeof set === 'string' ) {
			if ( window.moment ) {
				// Use moment if possible (even for ISO8601 strings, since it
				// will correctly handle 0000-00-00 and the like)
				var m = window.moment.utc( set, this.c.format, this.c.momentLocale, this.c.momentStrict );
				this.s.d = m.isValid() ? m.toDate() : null;
			}
			else {
				// Else must be using ISO8601 without moment (constructor would
				// have thrown an error otherwise)
				var match = set.match(/(\d{4})\-(\d{2})\-(\d{2})/ );
				this.s.d = match ?
					new Date( Date.UTC(match[1], match[2]-1, match[3]) ) :
					null;
			}
		}

		if ( write || write === undefined ) {
			if ( this.s.d ) {
				this._writeOutput();
			}
			else {
				// The input value was not valid...
				this.dom.input.val( set );
			}
		}

		// We need a date to be able to display the calendar at all
		if ( ! this.s.d ) {
			this.s.d = this._dateToUtc( new Date() );
		}

		this.s.display = new Date( this.s.d.toString() );

		// Set the day of the month to be 1 so changing between months doesn't
        // run into issues when going from day 31 to 28 (for example)
		this.s.display.setUTCDate( 1 );

		// Update the display elements for the new value
		this._setTitle();
		this._setCalander();
		this._setTime();
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */
	
	/**
	 * Build the control and assign initial event handlers
	 *
	 * @private
	 */
	_constructor: function () {
		var that = this;
		var classPrefix = this.c.classPrefix;
		var container = this.dom.container;
		var i18n = this.c.i18n;
		var onChange = this.c.onChange;

		if ( ! this.s.parts.date ) {
			this.dom.date.css( 'display', 'none' );
		}

		if ( ! this.s.parts.time ) {
			this.dom.time.css( 'display', 'none' );
		}

		if ( ! this.s.parts.seconds ) {
			this.dom.time.children('div.editor-datetime-timeblock').eq(2).remove();
			this.dom.time.children('span').eq(1).remove();
		}

		if ( ! this.s.parts.hours12 ) {
			this.dom.time.children('div.editor-datetime-timeblock').last().remove();
		}

		// Render the options
		this._optionsTitle();
		this._optionsTime( 'hours', this.s.parts.hours12 ? 12 : 24, 1 );
		this._optionsTime( 'minutes', 60, this.c.minutesIncrement );
		this._optionsTime( 'seconds', 60, this.c.secondsIncrement );
		this._options( 'ampm', [ 'am', 'pm' ], i18n.amPm );

		// Trigger the display of the widget when clicking or focusing on the
		// input element
		this.dom.input
			.on('focus.editor-datetime click.editor-datetime', function () {
				// If already visible - don't do anything
				if ( that.dom.container.is(':visible') || that.dom.input.is(':disabled') ) {
					return;
				}

				// In case the value has changed by text
				that.val( that.dom.input.val(), false );

				that._show();
			} )
			.on('keyup.editor-datetime', function () {
				// Update the calendar's displayed value as the user types
				if ( that.dom.container.is(':visible') ) {
					that.val( that.dom.input.val(), false );
				}
			} );

		// Main event handlers for input in the widget
		this.dom.container
			.on( 'change', 'select', function () {
				var select = $(this);
				var val = select.val();

				if ( select.hasClass(classPrefix+'-month') ) {
					// Month select
					that._correctMonth( that.s.display, val );
					that._setTitle();
					that._setCalander();
				}
				else if ( select.hasClass(classPrefix+'-year') ) {
					// Year select
					that.s.display.setUTCFullYear( val );
					that._setTitle();
					that._setCalander();
				}
				else if ( select.hasClass(classPrefix+'-hours') || select.hasClass(classPrefix+'-ampm') ) {
					// Hours - need to take account of AM/PM input if present
					if ( that.s.parts.hours12 ) {
						var hours = $(that.dom.container).find('.'+classPrefix+'-hours').val() * 1;
						var pm = $(that.dom.container).find('.'+classPrefix+'-ampm').val() === 'pm';

						that.s.d.setUTCHours( hours === 12 && !pm ?
							0 :
							pm && hours !== 12 ?
								hours + 12 :
								hours
						);
					}
					else {
						that.s.d.setUTCHours( val );
					}

					that._setTime();
					that._writeOutput( true );

					onChange();
				}
				else if ( select.hasClass(classPrefix+'-minutes') ) {
					// Minutes select
					that.s.d.setUTCMinutes( val );
					that._setTime();
					that._writeOutput( true );

					onChange();
				}
				else if ( select.hasClass(classPrefix+'-seconds') ) {
					// Seconds select
					that.s.d.setSeconds( val );
					that._setTime();
					that._writeOutput( true );

					onChange();
				}

				that.dom.input.focus();
				that._position();
			} )
			.on( 'click', function (e) {
				var nodeName = e.target.nodeName.toLowerCase();

				if ( nodeName === 'select' ) {
					return;
				}

				e.stopPropagation();

				if ( nodeName === 'button' ) {
					var button = $(e.target);
					var parent = button.parent();
					var select;

					if ( parent.hasClass('disabled') ) {
						return;
					}

					if ( parent.hasClass(classPrefix+'-iconLeft') ) {
						// Previous month
						that.s.display.setUTCMonth( that.s.display.getUTCMonth()-1 );
						that._setTitle();
						that._setCalander();

						that.dom.input.focus();
					}
					else if ( parent.hasClass(classPrefix+'-iconRight') ) {
						// Next month
						that._correctMonth( that.s.display, that.s.display.getUTCMonth()+1 );
						that._setTitle();
						that._setCalander();

						that.dom.input.focus();
					}
					else if ( parent.hasClass(classPrefix+'-iconUp') ) {
						// Value increase - common to all time selects
						select = parent.parent().find('select')[0];
						select.selectedIndex = select.selectedIndex !== select.options.length - 1 ?
							select.selectedIndex+1 :
							0;
						$(select).change();
					}
					else if ( parent.hasClass(classPrefix+'-iconDown') ) {
						// Value decrease - common to all time selects
						select = parent.parent().find('select')[0];
						select.selectedIndex = select.selectedIndex === 0 ?
							select.options.length - 1 :
							select.selectedIndex-1;
						$(select).change();
					}
					else {
						// Calendar click
						if ( ! that.s.d ) {
							that.s.d = that._dateToUtc( new Date() );
						}

						// Can't be certain that the current day will exist in
						// the new month, and likewise don't know that the
						// new day will exist in the old month, But 1 always
						// does, so we can change the month without worry of a
						// recalculation being done automatically by `Date`
						that.s.d.setUTCDate( 1 );
						that.s.d.setUTCFullYear( button.data('year') );
						that.s.d.setUTCMonth( button.data('month') );
						that.s.d.setUTCDate( button.data('day') );

						that._writeOutput( true );

						// Don't hide if there is a time picker, since we want to
						// be able to select a time as well.
						if ( ! that.s.parts.time ) {
							// This is annoying but IE has some kind of async
							// behaviour with focus and the focus from the above
							// write would occur after this hide - resulting in the
							// calendar opening immediately
							setTimeout( function () {
								that._hide();
							}, 10 );
						}
						else {
							that._setCalander();
						}

						onChange();
					}
				}
				else {
					// Click anywhere else in the widget - return focus to the
					// input element
					that.dom.input.focus();
				}
			} );
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private
	 */

	/**
	 * Compare the date part only of two dates - this is made super easy by the
	 * toDateString method!
	 *
	 * @param  {Date} a Date 1
	 * @param  {Date} b Date 2
	 * @private
	 */
	_compareDates: function( a, b ) {
		// Can't use toDateString as that converts to local time
		return this._dateToUtcString(a) === this._dateToUtcString(b);
	},

	/**
	 * When changing month, take account of the fact that some months don't have
	 * the same number of days. For example going from January to February you
	 * can have the 31st of Jan selected and just add a month since the date
	 * would still be 31, and thus drop you into March.
	 *
	 * @param  {Date} date  Date - will be modified
	 * @param  {integer} month Month to set
	 * @private
	 */
	_correctMonth: function ( date, month ) {
		var days = this._daysInMonth( date.getUTCFullYear(), month );
		var correctDays = date.getUTCDate() > days;

		date.setUTCMonth( month );

		if ( correctDays ) {
			date.setUTCDate( days );
			date.setUTCMonth( month );
		}
	},

	/**
	 * Get the number of days in a method. Based on
	 * http://stackoverflow.com/a/4881951 by Matti Virkkunen
	 *
	 * @param  {integer} year  Year
	 * @param  {integer} month Month (starting at 0)
	 * @private
	 */
	_daysInMonth: function ( year, month ) {
		// 
		var isLeap = ((year % 4) === 0 && ((year % 100) !== 0 || (year % 400) === 0));
		var months = [31, (isLeap ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

		return months[month];
	},

	/**
	 * Create a new date object which has the UTC values set to the local time.
	 * This allows the local time to be used directly for the library which
	 * always bases its calculations and display on UTC.
	 *
	 * @param  {Date} s Date to "convert"
	 * @return {Date}   Shifted date
	 */
	_dateToUtc: function ( s ) {
		return new Date( Date.UTC(
			s.getFullYear(), s.getMonth(), s.getDate(),
			s.getHours(), s.getMinutes(), s.getSeconds()
		) );
	},

	/**
	 * Create a UTC ISO8601 date part from a date object
	 *
	 * @param  {Date} d Date to "convert"
	 * @return {string} ISO formatted date
	 */
	_dateToUtcString: function ( d ) {
		return d.getUTCFullYear()+'-'+
			this._pad(d.getUTCMonth()+1)+'-'+
			this._pad(d.getUTCDate());
	},

	/**
	 * Hide the control and remove events related to its display
	 *
	 * @private
	 */
	_hide: function () {
		var namespace = this.s.namespace;

		this.dom.container.detach();

		$(window).off( '.'+namespace );
		$(document).off( 'keydown.'+namespace );
		$('div.DTE_Body_Content').off( 'scroll.'+namespace );
		$('body').off( 'click.'+namespace );
	},

	/**
	 * Convert a 24 hour value to a 12 hour value
	 *
	 * @param  {integer} val 24 hour value
	 * @return {integer}     12 hour value
	 * @private
	 */
	_hours24To12: function ( val ) {
		return val === 0 ?
			12 :
			val > 12 ?
				val - 12 :
				val;
	},

	/**
	 * Generate the HTML for a single day in the calendar - this is basically
	 * and HTML cell with a button that has data attributes so we know what was
	 * clicked on (if it is clicked on) and a bunch of classes for styling.
	 *
	 * @param  {object} day Day object from the `_htmlMonth` method
	 * @return {string}     HTML cell
	 */
	_htmlDay: function( day )
	{
		if ( day.empty ) {
			return '<td class="empty"></td>';
		}

		var classes = [ 'day' ];
		var classPrefix = this.c.classPrefix;

		if ( day.disabled ) {
			classes.push( 'disabled' );
		}

		if ( day.today ) {
			classes.push( 'today' );
		}

		if ( day.selected ) {
			classes.push( 'selected' );
		}

		return '<td data-day="' + day.day + '" class="' + classes.join(' ') + '">' +
				'<button class="'+classPrefix+'-button '+classPrefix+'-day" type="button" ' +'data-year="' + day.year + '" data-month="' + day.month + '" data-day="' + day.day + '">' +
						day.day +
				'</button>' +
			'</td>';
	},


	/**
	 * Create the HTML for a month to be displayed in the calendar table.
	 * 
	 * Based upon the logic used in Pikaday - MIT licensed
	 * Copyright (c) 2014 David Bushell
	 * https://github.com/dbushell/Pikaday
	 *
	 * @param  {integer} year  Year
	 * @param  {integer} month Month (starting at 0)
	 * @return {string} Calendar month HTML
	 * @private
	 */
	_htmlMonth: function ( year, month ) {
		var now    = this._dateToUtc( new Date() ),
			days   = this._daysInMonth( year, month ),
			before = new Date( Date.UTC(year, month, 1) ).getUTCDay(),
			data   = [],
			row    = [];

		if ( this.c.firstDay > 0 ) {
			before -= this.c.firstDay;

			if (before < 0) {
				before += 7;
			}
		}

		var cells = days + before,
			after = cells;

		while ( after > 7 ) {
			after -= 7;
		}

		cells += 7 - after;

		var minDate = this.c.minDate;
		var maxDate = this.c.maxDate;

		if ( minDate ) {
			minDate.setUTCHours(0);
			minDate.setUTCMinutes(0);
			minDate.setSeconds(0);
		}

		if ( maxDate ) {
			maxDate.setUTCHours(23);
			maxDate.setUTCMinutes(59);
			maxDate.setSeconds(59);
		}

		for ( var i=0, r=0 ; i<cells ; i++ ) {
			var day      = new Date( Date.UTC(year, month, 1 + (i - before)) ),
				selected = this.s.d ? this._compareDates(day, this.s.d) : false,
				today    = this._compareDates(day, now),
				empty    = i < before || i >= (days + before),
				disabled = (minDate && day < minDate) ||
				           (maxDate && day > maxDate);

			var disableDays = this.c.disableDays;
			if ( $.isArray( disableDays ) && $.inArray( day.getUTCDay(), disableDays ) !== -1 ) {
				disabled = true;
			}
			else if ( typeof disableDays === 'function' && disableDays( day ) === true ) {
				disabled = true;
			}

			var dayConfig = {
				day:      1 + (i - before),
				month:    month,
				year:     year,
				selected: selected,
				today:    today,
				disabled: disabled,
				empty:    empty
			};

			row.push( this._htmlDay(dayConfig) );

			if ( ++r === 7 ) {
				if ( this.c.showWeekNumber ) {
					row.unshift( this._htmlWeekOfYear(i - before, month, year) );
				}

				data.push( '<tr>'+row.join('')+'</tr>' );
				row = [];
				r = 0;
			}
		}

		var className = this.c.classPrefix+'-table';
		if ( this.c.showWeekNumber ) {
			className += ' weekNumber';
		}

		return '<table class="'+className+'">' +
				'<thead>'+
					this._htmlMonthHead() +
				'</thead>'+
				'<tbody>'+
					data.join('') +
				'</tbody>'+
			'</table>';
	},

	/**
	 * Create the calendar table's header (week days)
	 *
	 * @return {string} HTML cells for the row
	 * @private
	 */
	_htmlMonthHead: function () {
		var a = [];
		var firstDay = this.c.firstDay;
		var i18n = this.c.i18n;

		// Take account of the first day shift
		var dayName = function ( day ) {
			day += firstDay;

			while (day >= 7) {
				day -= 7;
			}

			return i18n.weekdays[day];
		};
		
		// Empty cell in the header
		if ( this.c.showWeekNumber ) {
			a.push( '<th></th>' );
		}

		for ( var i=0 ; i<7 ; i++ ) {
			a.push( '<th>'+dayName( i )+'</th>' );
		}

		return a.join('');
	},

	/**
	 * Create a cell that contains week of the year - ISO8601
	 *
	 * Based on https://stackoverflow.com/questions/6117814/ and
	 * http://techblog.procurios.nl/k/n618/news/view/33796/14863/
	 *
	 * @param  {integer} d Day of month
	 * @param  {integer} m Month of year (zero index)
	 * @param  {integer} y Year
	 * @return {string}   
	 * @private
	 */
	_htmlWeekOfYear: function ( d, m, y ) {
		var date = new Date( y, m, d, 0, 0, 0, 0 );

		// First week of the year always has 4th January in it
		date.setDate( date.getDate() + 4 - (date.getDay() || 7) );

		var oneJan = new Date( y, 0, 1 );
		var weekNum = Math.ceil( ( ( (date - oneJan) / 86400000) + 1)/7 );

		return '<td class="'+this.c.classPrefix+'-week">' + weekNum + '</td>';
	},

	/**
	 * Create option elements from a range in an array
	 *
	 * @param  {string} selector Class name unique to the select element to use
	 * @param  {array} values   Array of values
	 * @param  {array} [labels] Array of labels. If given must be the same
	 *   length as the values parameter.
	 * @private
	 */
	_options: function ( selector, values, labels ) {
		if ( ! labels ) {
			labels = values;
		}

		var select = this.dom.container.find('select.'+this.c.classPrefix+'-'+selector);
		select.empty();

		for ( var i=0, ien=values.length ; i<ien ; i++ ) {
			select.append( '<option value="'+values[i]+'">'+labels[i]+'</option>' );
		}
	},

	/**
	 * Set an option and update the option's span pair (since the select element
	 * has opacity 0 for styling)
	 *
	 * @param  {string} selector Class name unique to the select element to use
	 * @param  {*}      val      Value to set
	 * @private
	 */
	_optionSet: function ( selector, val ) {
		var select = this.dom.container.find('select.'+this.c.classPrefix+'-'+selector);
		var span = select.parent().children('span');

		select.val( val );

		var selected = select.find('option:selected');
		span.html( selected.length !== 0 ?
			selected.text() :
			this.c.i18n.unknown
		);
	},

	/**
	 * Create time option list. Can't just use `_options` for the time as the
	 * hours can be a little complex and we want to be able to control the
	 * increment option for the minutes and seconds.
	 *
	 * @param  {jQuery} select Select element to operate on
	 * @param  {integer} count  Value to loop to
	 * @param  {integer} inc    Increment value
	 * @private
	 */
	_optionsTime: function ( select, count, inc ) {
		var classPrefix = this.c.classPrefix;
		var sel = this.dom.container.find('select.'+classPrefix+'-'+select);
		var start=0, end=count;
		var allowed = this.c.hoursAvailable;
		var render = count === 12 ?
			function (i) { return i; } :
			this._pad;

		if ( count === 12 ) {
			start = 1;
			end = 13;
		}

		for ( var i=start ; i<end ; i+=inc ) {
			if ( ! allowed || $.inArray( i, allowed ) !== -1 ) {
				sel.append( '<option value="'+i+'">'+render(i)+'</option>' );
			}
		}
	},

	/**
	 * Create the options for the month and year
	 *
	 * @param  {integer} year  Year
	 * @param  {integer} month Month (starting at 0)
	 * @private
	 */
	_optionsTitle: function ( year, month ) {
		var classPrefix = this.c.classPrefix;
		var i18n = this.c.i18n;
		var min = this.c.minDate;
		var max = this.c.maxDate;
		var minYear = min ? min.getFullYear() : null;
		var maxYear = max ? max.getFullYear() : null;

		var i = minYear !== null ? minYear : new Date().getFullYear() - this.c.yearRange;
		var j = maxYear !== null ? maxYear : new Date().getFullYear() + this.c.yearRange;

		this._options( 'month', this._range( 0, 11 ), i18n.months );
		this._options( 'year', this._range( i, j ) );
	},

	/**
	 * Simple two digit pad
	 *
	 * @param  {integer} i      Value that might need padding
	 * @return {string|integer} Padded value
	 * @private
	 */
	_pad: function ( i ) {
		return i<10 ? '0'+i : i;
	},

	/**
	 * Position the calendar to look attached to the input element
	 * @private
	 */
	_position: function () {
		var offset = this.dom.input.offset();
		var container = this.dom.container;
		var inputHeight = this.dom.input.outerHeight();

		container
			.css( {
				top: offset.top + inputHeight,
				left: offset.left
			} )
			.appendTo( 'body' );

		var calHeight = container.outerHeight();
		var calWidth = container.outerWidth();
		var scrollTop = $(window).scrollTop();

		// Correct to the bottom
		if ( offset.top + inputHeight + calHeight - scrollTop > $(window).height() ) {
			var newTop = offset.top - calHeight;

			container.css( 'top', newTop < 0 ? 0 : newTop );
		}

		// Correct to the right
		if ( calWidth + offset.left > $(window).width() ) {
			var newLeft = $(window).width() - calWidth;

			container.css( 'left', newLeft < 0 ? 0 : newLeft );
		}
	},

	/**
	 * Create a simple array with a range of values
	 *
	 * @param  {integer} start Start value (inclusive)
	 * @param  {integer} end   End value (inclusive)
	 * @return {array}         Created array
	 * @private
	 */
	_range: function ( start, end ) {
		var a = [];

		for ( var i=start ; i<=end ; i++ ) {
			a.push( i );
		}

		return a;
	},

	/**
	 * Redraw the calendar based on the display date - this is a destructive
	 * operation
	 *
	 * @private
	 */
	_setCalander: function () {
		if ( this.s.display ) {
			this.dom.calendar
				.empty()
				.append( this._htmlMonth(
					this.s.display.getUTCFullYear(),
					this.s.display.getUTCMonth()
				) );
		}
	},

	/**
	 * Set the month and year for the calendar based on the current display date
	 *
	 * @private
	 */
	_setTitle: function () {
		this._optionSet( 'month', this.s.display.getUTCMonth() );
		this._optionSet( 'year', this.s.display.getUTCFullYear() );
	},

	/**
	 * Set the time based on the current value of the widget
	 *
	 * @private
	 */
	_setTime: function () {
		var d = this.s.d;
		var hours = d ? d.getUTCHours() : 0;

		if ( this.s.parts.hours12 ) {
			this._optionSet( 'hours', this._hours24To12( hours ) );
			this._optionSet( 'ampm', hours < 12 ? 'am' : 'pm' );
		}
		else {
			this._optionSet( 'hours', hours );
		}

		this._optionSet( 'minutes', d ? d.getUTCMinutes() : 0 );
		this._optionSet( 'seconds', d ? d.getSeconds() : 0 );
	},

	/**
	 * Show the widget and add events to the document required only while it
	 * is displayed
	 * 
	 * @private
	 */
	_show: function () {
		var that = this;
		var namespace = this.s.namespace;

		this._position();

		// Need to reposition on scroll
		$(window).on( 'scroll.'+namespace+' resize.'+namespace, function () {
			that._position();
		} );

		$('div.DTE_Body_Content').on( 'scroll.'+namespace, function () {
			that._position();
		} );

		// On tab focus will move to a different field (no keyboard navigation
		// in the date picker - this might need to be changed).
		// On esc the Editor might close. Even if it doesn't the date picker
		// should
		$(document).on( 'keydown.'+namespace, function (e) {
			if (
				e.keyCode === 9  || // tab
				e.keyCode === 27 || // esc
				e.keyCode === 13    // return
			) {
				that._hide();
			}
		} );

		// Hide if clicking outside of the widget - but in a different click
		// event from the one that was used to trigger the show (bubble and
		// inline)
		setTimeout( function () {
			$('body').on( 'click.'+namespace, function (e) {
				var parents = $(e.target).parents();

				if ( ! parents.filter( that.dom.container ).length && e.target !== that.dom.input[0] ) {
					that._hide();
				}
			} );
		}, 10 );
	},

	/**
	 * Write the formatted string to the input element this control is attached
	 * to
	 *
	 * @private
	 */
	_writeOutput: function ( focus ) {
		var date = this.s.d;

		// Use moment if possible - otherwise it must be ISO8601 (or the
		// constructor would have thrown an error)
		var out = window.moment ?
			window.moment.utc( date, undefined, this.c.momentLocale, this.c.momentStrict ).format( this.c.format ) :
			date.getUTCFullYear() +'-'+
	            this._pad(date.getUTCMonth() + 1) +'-'+
	            this._pad(date.getUTCDate());
		
		this.dom.input.val( out );

		if ( focus ) {
			this.dom.input.focus();
		}
	}
} );


/**
 * For generating unique namespaces
 *
 * @type {Number}
 * @private
 */
Editor.DateTime._instance = 0;

/**
 * Defaults for the date time picker
 *
 * @type {Object}
 */
Editor.DateTime.defaults = {
	// Not documented - could be an internal property
	classPrefix: 'editor-datetime',

	// function or array of ints
	disableDays: null,

	// first day of the week (0: Sunday, 1: Monday, etc)
	firstDay: 1,

	format: 'YYYY-MM-DD',

	hoursAvailable: null,

	// Not documented as i18n is done by the Editor.defaults.i18n obj
	i18n: Editor.defaults.i18n.datetime,

	maxDate: null,

	minDate: null,

	minutesIncrement: 1,

	momentStrict: true,

	momentLocale: 'en',

	onChange: function () {},

	secondsIncrement: 1,

	// show the ISO week number at the head of the row
	showWeekNumber: false,

	// overruled by max / min date
	yearRange: 10
};



(function() {

var fieldTypes = Editor.fieldTypes;

// Upload private helper method
function _buttonText ( conf, text )
{
	if ( text === null || text === undefined ) {
		text = conf.uploadText || "Choose file...";
	}

	conf._input.find('div.upload button').html( text );
}

function _commonUpload ( editor, conf, dropCallback )
{
	var btnClass = editor.classes.form.button;
	var container = $(
		'<div class="editor_upload">'+
			'<div class="eu_table">'+
				'<div class="row">'+
					'<div class="cell upload">'+
						'<button class="'+btnClass+'" />'+
						'<input type="file"/>'+
					'</div>'+
					'<div class="cell clearValue">'+
						'<button class="'+btnClass+'" />'+
					'</div>'+
				'</div>'+
				'<div class="row second">'+
					'<div class="cell">'+
						'<div class="drop"><span/></div>'+
					'</div>'+
					'<div class="cell">'+
						'<div class="rendered"/>'+
					'</div>'+
				'</div>'+
			'</div>'+
		'</div>'
	);

	conf._input = container;
	conf._enabled = true;

	_buttonText( conf );

	if ( window.FileReader && conf.dragDrop !== false ) {
		container.find('div.drop span').text(
			conf.dragDropText || "Drag and drop a file here to upload"
		);

		var dragDrop = container.find('div.drop');
		dragDrop
			.on( 'drop', function (e) {
				if ( conf._enabled ) {
					Editor.upload( editor, conf, e.originalEvent.dataTransfer.files, _buttonText, dropCallback );
					dragDrop.removeClass('over');
				}
				return false;
			} )
			.on( 'dragleave dragexit', function (e) {
				if ( conf._enabled ) {
					dragDrop.removeClass('over');
				}
				return false;
			} )
			.on( 'dragover', function (e) {
				if ( conf._enabled ) {
					dragDrop.addClass('over');
				}
				return false;
			} );

		// When an Editor is open with a file upload input there is a
		// reasonable chance that the user will miss the drop point when
		// dragging and dropping. Rather than loading the file in the browser,
		// we want nothing to happen, otherwise the form will be lost.
		editor
			.on( 'open', function () {
				$('body').on( 'dragover.DTE_Upload drop.DTE_Upload', function (e) {
					return false;
				} );
			} )
			.on( 'close', function () {
				$('body').off( 'dragover.DTE_Upload drop.DTE_Upload' );
			} );
	}
	else {
		container.addClass( 'noDrop' );
		container.append( container.find('div.rendered') );
	}

	container.find('div.clearValue button').on( 'click', function () {
		Editor.fieldTypes.upload.set.call( editor, conf, '' );
	} );

	container.find('input[type=file]').on('change', function () {
		Editor.upload( editor, conf, this.files, _buttonText, function (ids) {
			dropCallback.call( editor, ids );

			// Clear the value so change will happen on the next file select,
			// even if it is the same file
			container.find('input[type=file]').val('');
		} );
	} );

	return container;
}

// Typically a change event caused by the end user will be added to a queue that
// the browser will handle when no other script is running. However, using
// `$().trigger()` will cause it to happen immediately, so in order to simulate
// the standard browser behaviour we use setTimeout. This also means that
// `dependent()` and other change event listeners will trigger when the field
// values have all been set, rather than as they are being set - 31594
function _triggerChange ( input ) {
	setTimeout( function () {
		input.trigger( 'change', {editor: true, editorSet: true} ); // editorSet legacy
	}, 0 );
}


// A number of the fields in this file use the same get, set, enable and disable
// methods (specifically the text based controls), so in order to reduce the code
// size, we just define them once here in our own local base model for the field
// types.
var baseFieldType = $.extend( true, {}, Editor.models.fieldType, {
	get: function ( conf ) {
		return conf._input.val();
	},

	set: function ( conf, val ) {
		conf._input.val( val );
		_triggerChange( conf._input );
	},

	enable: function ( conf ) {
		conf._input.prop( 'disabled', false );
	},

	disable: function ( conf ) {
		conf._input.prop( 'disabled', true );
	},

	canReturnSubmit: function ( conf, node ) {
		return true;
	}
} );



fieldTypes.hidden = {
	create: function ( conf ) {
		conf._val = conf.value;
		return null;
	},

	get: function ( conf ) {
		return conf._val;
	},

	set: function ( conf, val ) {
		conf._val = val;
	}
};


fieldTypes.readonly = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<input/>').attr( $.extend( {
			id: Editor.safeId( conf.id ),
			type: 'text',
			readonly: 'readonly'
		}, conf.attr || {} ) );

		return conf._input[0];
	}
} );


fieldTypes.text = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<input/>').attr( $.extend( {
			id: Editor.safeId( conf.id ),
			type: 'text'
		}, conf.attr || {} ) );

		return conf._input[0];
	}
} );


fieldTypes.password = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<input/>').attr( $.extend( {
			id: Editor.safeId( conf.id ),
			type: 'password'
		}, conf.attr || {} ) );

		return conf._input[0];
	}
} );

fieldTypes.textarea = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<textarea/>').attr( $.extend( {
			id: Editor.safeId( conf.id )
		}, conf.attr || {} ) );
		return conf._input[0];
	},

	canReturnSubmit: function ( conf, node ) {
		return false;
	}
} );


fieldTypes.select = $.extend( true, {}, baseFieldType, {
	// Locally "private" function that can be reused for the create and update methods
	_addOptions: function ( conf, opts, append ) {
		var elOpts = conf._input[0].options;
		var countOffset = 0;

		if ( ! append ) {
			elOpts.length = 0;

			if ( conf.placeholder !== undefined ) {
				var placeholderValue = conf.placeholderValue !== undefined ?
					conf.placeholderValue :
					'';

				countOffset += 1;
				elOpts[0] = new Option( conf.placeholder, placeholderValue );

				var disabled = conf.placeholderDisabled !== undefined ?
					conf.placeholderDisabled :
					true;

				elOpts[0].hidden = disabled; // can't be hidden if not disabled!
				elOpts[0].disabled = disabled;
				elOpts[0]._editor_val = placeholderValue;
			}
		}
		else {
			countOffset = elOpts.length;
		}

		if ( opts ) {
			Editor.pairs( opts, conf.optionsPair, function ( val, label, i, attr ) {
				var option = new Option( label, val );
				option._editor_val = val;

				if ( attr ) {
					$(option).attr( attr );
				}

				elOpts[ i+countOffset ] = option;
			} );
		}
	},

	create: function ( conf ) {
		conf._input = $('<select/>')
			.attr( $.extend( {
				id: Editor.safeId( conf.id ),
				multiple: conf.multiple === true
			}, conf.attr || {} ) )
			.on( 'change.dte', function (e, d) {
				// On change, get the user selected value and store it as the
				// last set, so `update` can reflect it. This way `_lastSet`
				// always gives the intended value, be it set via the API or by
				// the end user.
				if ( ! d || ! d.editor ) {
					conf._lastSet = fieldTypes.select.get( conf );
				}
			} );

		fieldTypes.select._addOptions( conf, conf.options || conf.ipOpts );

		return conf._input[0];
	},

	update: function ( conf, options, append ) {
		fieldTypes.select._addOptions( conf, options, append );

		// Attempt to set the last selected value (set by the API or the end
		// user, they get equal priority)
		var lastSet = conf._lastSet;

		if ( lastSet !== undefined ) {
			fieldTypes.select.set( conf, lastSet, true );
		}

		_triggerChange( conf._input );
	},

	get: function ( conf ) {
		var val = conf._input.find('option:selected').map( function () {
			return this._editor_val;
		} ).toArray();

		if ( conf.multiple ) {
			return conf.separator ?
				val.join( conf.separator ) :
				val;
		}

		return val.length ? val[0] : null;
	},

	set: function ( conf, val, localUpdate ) {
		if ( ! localUpdate ) {
			conf._lastSet = val;
		}

		// Can't just use `$().val()` because it won't work with strong types
		if ( conf.multiple && conf.separator && ! $.isArray( val ) ) {
			val = typeof val === 'string' ?
				val.split( conf.separator ) :
				[];
		}
		else if ( ! $.isArray( val ) ) {
			val = [ val ];
		}

		var i, len=val.length, found, allFound = false;
		var options = conf._input.find('option');

		conf._input.find('option').each( function () {
			found = false;

			for ( i=0 ; i<len ; i++ ) {
				// Weak typing
				if ( this._editor_val == val[i] ) {
					found = true;
					allFound = true;
					break;
				}
			}

			this.selected = found;
		} );

		// If there is a placeholder, we might need to select it if nothing else
		// was selected. It doesn't make sense to select when multi is enabled
		if ( conf.placeholder && ! allFound && ! conf.multiple && options.length ) {
			options[0].selected = true;
		}

		// Update will call change itself, otherwise multiple might be called
		if ( ! localUpdate ) {
			_triggerChange( conf._input );
		}

		return allFound;
	},

	destroy: function ( conf ) {
		conf._input.off( 'change.dte' );
	}
} );


fieldTypes.checkbox = $.extend( true, {}, baseFieldType, {
	// Locally "private" function that can be reused for the create and update methods
	_addOptions: function ( conf, opts, append ) {
		var val, label;
		var jqInput = conf._input;
		var offset = 0;

		if ( ! append ) {
			jqInput.empty();
		}
		else {
			offset = $('input', jqInput).length;
		}

		if ( opts ) {
			Editor.pairs( opts, conf.optionsPair, function ( val, label, i, attr ) {
				jqInput.append(
					'<div>'+
						'<input id="'+Editor.safeId( conf.id )+'_'+(i+offset)+'" type="checkbox" />'+
						'<label for="'+Editor.safeId( conf.id )+'_'+(i+offset)+'">'+label+'</label>'+
					'</div>'
				);
				$('input:last', jqInput).attr('value', val)[0]._editor_val = val;

				if ( attr ) {
					$('input:last', jqInput).attr( attr );
				}
			} );
		}
	},


	create: function ( conf ) {
		conf._input = $('<div />');
		fieldTypes.checkbox._addOptions( conf, conf.options || conf.ipOpts );

		return conf._input[0];
	},

	get: function ( conf ) {
		var out = [];
		var selected = conf._input.find('input:checked');

		if ( selected.length ) {
			selected.each( function () {
				out.push( this._editor_val );
			} );
		}
		else if ( conf.unselectedValue !== undefined ) {
			out.push( conf.unselectedValue );
		}

		return conf.separator === undefined || conf.separator === null ?
			out :
			out.join(conf.separator);
	},

	set: function ( conf, val ) {
		var jqInputs = conf._input.find('input');
		if ( ! $.isArray(val) && typeof val === 'string' ) {
			val = val.split( conf.separator || '|' );
		}
		else if ( ! $.isArray(val) ) {
			val = [ val ];
		}

		var i, len=val.length, found;

		jqInputs.each( function () {
			found = false;

			for ( i=0 ; i<len ; i++ ) {
				if ( this._editor_val == val[i] ) {
					found = true;
					break;
				}
			}

			this.checked = found;
		} );

		_triggerChange( jqInputs );
	},

	enable: function ( conf ) {
		conf._input.find('input').prop('disabled', false);
	},

	disable: function ( conf ) {
		conf._input.find('input').prop('disabled', true);
	},

	update: function ( conf, options, append ) {
		// Get the current value
		var checkbox = fieldTypes.checkbox;
		var currVal = checkbox.get( conf );

		checkbox._addOptions( conf, options, append );
		checkbox.set( conf, currVal );
	}
} );


fieldTypes.radio = $.extend( true, {}, baseFieldType, {
	// Locally "private" function that can be reused for the create and update methods
	_addOptions: function ( conf, opts, append ) {
		var val, label;
		var jqInput = conf._input;
		var offset = 0;

		if ( ! append ) {
			jqInput.empty();
		}
		else {
			offset = $('input', jqInput).length;
		}

		if ( opts ) {
			Editor.pairs( opts, conf.optionsPair, function ( val, label, i, attr ) {
				jqInput.append(
					'<div>'+
						'<input id="'+Editor.safeId( conf.id )+'_'+(i+offset)+'" type="radio" name="'+conf.name+'" />'+
						'<label for="'+Editor.safeId( conf.id )+'_'+(i+offset)+'">'+label+'</label>'+
					'</div>'
				);
				$('input:last', jqInput).attr('value', val)[0]._editor_val = val;

				if ( attr ) {
					$('input:last', jqInput).attr( attr );
				}
			} );
		}
	},


	create: function ( conf ) {
		conf._input = $('<div />');
		fieldTypes.radio._addOptions( conf, conf.options || conf.ipOpts );

		// this is ugly, but IE6/7 has a problem with radio elements that are created
		// and checked before being added to the DOM! Basically it doesn't check them. As
		// such we use the _preChecked property to set cache the checked button and then
		// check it again when the display is shown. This has no effect on other browsers
		// other than to cook a few clock cycles.
		this.on('open', function () {
			conf._input.find('input').each( function () {
				if ( this._preChecked ) {
					this.checked = true;
				}
			} );
		} );

		return conf._input[0];
	},

	get: function ( conf ) {
		var el = conf._input.find('input:checked');
		return el.length ? el[0]._editor_val : undefined;
	},

	set: function ( conf, val ) {
		var that  = this;

		conf._input.find('input').each( function () {
			this._preChecked = false;

			if ( this._editor_val == val ) {
				this.checked = true;
				this._preChecked = true;
			}
			else {
				// In a detached DOM tree, there is no relationship between the
				// input elements, so we need to uncheck any element that does
				// not match the value
				this.checked = false;
				this._preChecked = false;
			}
		} );

		_triggerChange( conf._input.find('input:checked') );
	},

	enable: function ( conf ) {
		conf._input.find('input').prop('disabled', false);
	},

	disable: function ( conf ) {
		conf._input.find('input').prop('disabled', true);
	},

	update: function ( conf, options, append ) {
		var radio = fieldTypes.radio;
		var currVal = radio.get( conf );

		radio._addOptions( conf, options, append );

		// Select the current value if it exists in the new data set, otherwise
		// select the first radio input so there is always a value selected
		var inputs = conf._input.find('input');
		radio.set( conf, inputs.filter('[value="'+currVal+'"]').length ?
			currVal :
			inputs.eq(0).attr('value')
		);
	}
} );


fieldTypes.date = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<input />').attr( $.extend( {
			id: Editor.safeId( conf.id ),
			type: 'text'
		}, conf.attr ) );

		if ( $.datepicker ) {
			// jQuery UI date picker
			conf._input.addClass( 'jqueryui' );

			if ( ! conf.dateFormat ) {
				conf.dateFormat = $.datepicker.RFC_2822;
			}

			// Allow the element to be attached to the DOM
			setTimeout( function () {
				$( conf._input ).datepicker( $.extend( {
					showOn: "both",
					dateFormat: conf.dateFormat,
					buttonImage: conf.dateImage,
					buttonImageOnly: true,
					onSelect: function () {
						conf._input.focus().click();
					}
				}, conf.opts ) );

				$('#ui-datepicker-div').css('display','none');
			}, 10 );
		}
		else {
			// HTML5 (only Chrome and Edge on the desktop support this atm)
			conf._input.attr( 'type', 'date' );
		}

		return conf._input[0];
	},

	// use default get method as will work for all

	set: function ( conf, val ) {
		if ( $.datepicker && conf._input.hasClass('hasDatepicker') ) {
			// Due to the async init of the control it is possible that we might
			// try to set a value before it has been initialised!
			conf._input.datepicker( "setDate" , val ).change();
		}
		else {
			$(conf._input).val( val );
		}
	},

	enable: function ( conf ) {
		if ( $.datepicker ) {
			conf._input.datepicker( "enable" );
		}
		else {
			$(conf._input).prop( 'disabled', false );
		}
	},

	disable: function ( conf ) {
		if ( $.datepicker ) {
			conf._input.datepicker( "disable" );
		}
		else {
			$(conf._input).prop( 'disabled', true );
		}
	},

	owns: function ( conf, node ) {
		return $(node).parents('div.ui-datepicker').length || $(node).parents('div.ui-datepicker-header').length ?
			true :
			false;
	}
} );


fieldTypes.datetime = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		conf._input = $('<input />').attr( $.extend( true, {
			id:   Editor.safeId( conf.id ),
			type: 'text'
		}, conf.attr ) );

		conf._picker = new Editor.DateTime( conf._input, $.extend( {
			format: conf.format, // can be undefined
			i18n: this.i18n.datetime,
			onChange: function () {
				_triggerChange( conf._input );
			}
		}, conf.opts ) );

		conf._closeFn = function () {
			conf._picker.hide();
		};
		this.on( 'close', conf._closeFn );

		return conf._input[0];
	},

	// default get, disable and enable options are okay

	set: function ( conf, val ) {
		conf._picker.val( val );

		_triggerChange( conf._input );
	},

	owns: function ( conf, node ) {
		return conf._picker.owns( node );
	},

	errorMessage: function ( conf, msg ) {
		conf._picker.errorMsg( msg );
	},

	destroy: function ( conf ) {
		this.off( 'close', conf._closeFn );
		conf._picker.destroy();
	},

	minDate: function ( conf, min ) {
		conf._picker.min( min );
	},

	maxDate: function ( conf, max ) {
		conf._picker.max( max );
	}
} );


fieldTypes.upload = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		var editor = this;
		var container = _commonUpload( editor, conf, function ( val ) {
			Editor.fieldTypes.upload.set.call( editor, conf, val[0] );
		} );

		return container;
	},

	get: function ( conf ) {
		return conf._val;
	},

	set: function ( conf, val ) {
		conf._val = val;

		var container = conf._input;

		if ( conf.display ) {
			var rendered = container.find('div.rendered');

			if ( conf._val ) {
				rendered.html( conf.display( conf._val ) );
			}
			else {
				rendered
					.empty()
					.append( '<span>'+( conf.noFileText || 'No file' )+'</span>' );
			}
		}

		var button = container.find('div.clearValue button');
		if ( val && conf.clearText ) {
			button.html( conf.clearText );
			container.removeClass( 'noClear' );
		}
		else {
			container.addClass( 'noClear' );
		}

		conf._input.find('input').triggerHandler( 'upload.editor', [ conf._val ] );
	},

	enable: function ( conf ) {
		conf._input.find('input').prop('disabled', false);
		conf._enabled = true;
	},

	disable: function ( conf ) {
		conf._input.find('input').prop('disabled', true);
		conf._enabled = false;
	},

	canReturnSubmit: function ( conf, node ) {
		return false;
	}
} );


fieldTypes.uploadMany = $.extend( true, {}, baseFieldType, {
	create: function ( conf ) {
		var editor = this;
		var container = _commonUpload( editor, conf, function ( val ) {
			conf._val = conf._val.concat( val );
			Editor.fieldTypes.uploadMany.set.call( editor, conf, conf._val );
		} );

		container
			.addClass( 'multi' )
			.on( 'click', 'button.remove', function (e) {
				e.stopPropagation();

				var idx = $(this).data('idx');

				conf._val.splice( idx, 1 );
				Editor.fieldTypes.uploadMany.set.call( editor, conf, conf._val );
			} );

		return container;
	},

	get: function ( conf ) {
		return conf._val;
	},

	set: function ( conf, val ) {
		// Default value for fields is an empty string, whereas we want []
		if ( ! val ) {
			val = [];
		}

		if ( ! $.isArray( val ) ) {
			throw 'Upload collections must have an array as a value';
		}

		conf._val = val;

		var that = this;
		var container = conf._input;

		if ( conf.display ) {
			var rendered = container.find('div.rendered').empty();
			
			if ( val.length ) {
				var list = $('<ul/>').appendTo( rendered );

				$.each( val, function ( i, file ) {
					list.append(
						'<li>'+
							conf.display( file, i )+
							' <button class="'+that.classes.form.button+' remove" data-idx="'+i+'">&times;</button>'+
						'</li>'
					);
				} );
			}
			else {
				rendered.append( '<span>'+( conf.noFileText || 'No files' )+'</span>' );
			}
		}

		conf._input.find('input').triggerHandler( 'upload.editor', [ conf._val ] );
	},

	enable: function ( conf ) {
		conf._input.find('input').prop('disabled', false);
		conf._enabled = true;
	},

	disable: function ( conf ) {
		conf._input.find('input').prop('disabled', true);
		conf._enabled = false;
	},

	canReturnSubmit: function ( conf, node ) {
		return false;
	}
} );


}());


// If there are field types available on DataTables we copy them in (after the
// built in ones to allow overrides) and then expose the field types object.
if ( DataTable.ext.editorFields ) {
	$.extend( Editor.fieldTypes, DataTable.ext.editorFields );
}

DataTable.ext.editorFields = Editor.fieldTypes;


/**
 * File information for uploads
 */
Editor.files = {};


/**
 * Name of this class
 *  @constant CLASS
 *  @type     String
 *  @default  Editor
 */
Editor.prototype.CLASS = "Editor";


/**
 * DataTables Editor version
 *  @constant  Editor.VERSION
 *  @type      String
 *  @default   See code
 *  @static
 */
Editor.version = "1.7.0";


// Event documentation for JSDoc
/**
 * Processing event, fired when Editor submits data to the server for processing.
 * This can be used to provide your own processing indicator if your UI framework
 * already has one.
 *  @name Editor#processing
 *  @event
 *  @param {event} e jQuery event object
 *  @param {boolean} processing Flag for if the processing is running (true) or
 *    not (false).
 */

/**
 * Form displayed event, fired when the form is made available in the DOM. This
 * can be useful for fields that require height and width calculations to be
 * performed since the element is not available in the document until the
 * form is displayed.
 *  @name Editor#open
 *  @event
 *  @param {event} e jQuery event object
 *  @param {string} type Editing type
 */

/**
 * Before a form is displayed, this event is fired. It allows the open action to be
 * cancelled by returning false from the function.
 *  @name Editor#preOpen
 *  @event
 *  @param {event} e jQuery event object
 */

/**
 * Form hidden event, fired when the form is removed from the document. The
 * of the compliment `open` event.
 *  @name Editor#close
 *  @event
 *  @param {event} e jQuery event object
 */

/**
 * Before a form is closed, this event is fired. It allows the close action to be
 * cancelled by returning false from the function. This can be useful for confirming
 * that the user actually wants to close the display (if they have unsaved changes
 * for example).
 *  @name Editor#preClose
 *  @event
 *  @param {event} e jQuery event object
 *  @param {string} trigger Action that caused the close event - can be undefined.
 *    Typically defined by the display controller.
 */

/**
 * Emitted before a form blur occurs. A form blur is similar to a close, but
 * is triggered by a user, typically, clicking on the background, while a close
 * occurs due to a click on the close button. A blur can precede a close.
 *  @name Editor#preBlur
 *  @event
 *  @param {event} e jQuery event object
 */

/**
 * Pre-submit event for the form, fired just before the data is submitted to
 * the server. This event allows you to modify the data that will be submitted
 * to the server. Note that this event runs after the 'formatdata' callback
 * function of the {@link Editor#submit} API method.
 *  @name Editor#preSubmit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} data The data object that will be submitted to the server
 *  @param {string} action The action type for this submit - `create`, `edit` or
 *    `remove`.
 */

/**
 * Post-submit event for the form, fired immediately after the data has been
 * loaded by the Ajax call, allowing modification or any other interception
 * of the data returned form the server.
 *  @name Editor#postSubmit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data object that was be submitted to the server
 *  @param {string} action The action type for this submit - `create`, `edit` or
 *    `remove`.
 */

/**
 * Submission complete event, fired when data has been submitted to the server and
 * after any of the return handling code has been run (updating the DataTable
 * for example). Note that unlike `submitSuccess` and `submitError`, `submitComplete`
 * will be fired for both a successful submission and an error. Additionally this
 * event will be fired after `submitSuccess` or `submitError`.
 *  @name Editor#submitComplete
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * Submission complete and successful event, fired when data has been successfully
 * submitted to the server and all actions required by the returned data (inserting
 * or updating a row) have been completed.
 *  @name Editor#submitSuccess
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * Submission complete, but in error event, fired when data has been submitted to
 * the server but an error occurred on the server (typically a JSON formatting error)
 *  @name Editor#submitError
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} xhr The Ajax object
 *  @param {string} err The error message from jQuery
 *  @param {object} thrown The exception thrown by jQuery
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * Create method activated event, fired when the create API method has been called,
 * just prior to the form being shown. Useful for manipulating the form specifically
 * for the create state.
 *  @name Editor#initCreate
 *  @event
 *  @param {event} e jQuery event object
 */

/**
 * Pre-create new row event, fired just before DataTables calls the fnAddData method
 * to add new data to the DataTable, allowing modification of the data that will be
 * used to insert into the table.
 *  @name Editor#preCreate
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that will be used to update the DataTable
 */

/**
 * Create new row event, fired when a new row has been created in the DataTable by
 * a form submission. This is called just after the fnAddData call to the DataTable.
 *  @name Editor#create
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * As per the `create` event - included for naming consistency.
 *  @name Editor#postCreate
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * Edit method activated event, fired when the edit API method has been called,
 * just prior to the form being shown. Useful for manipulating the form specifically
 * for the edit state.
 *  @name Editor#initEdit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {node} tr TR element of the row to be edited
 *  @param {array|object} data Data source array / object for the row to be
 *    edited
 */

/**
 * Pre-edit row event, fired just before DataTables calls the fnUpdate method
 * to edit data in a DataTables row, allowing modification of the data that will be
 * used to update the table.
 *  @name Editor#preEdit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that will be used to update the DataTable
 */

/**
 * Edit row event, fired when a row has been edited in the DataTable by a form
 * submission. This is called just after the fnUpdate call to the DataTable.
 *  @name Editor#edit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * As per the `edit` event - included for naming consistency.
 *  @name Editor#postEdit
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that was used to update the DataTable
 */

/**
 * Remove method activated event, fired when the remove API method has been
 * called, just prior to the form being shown. Useful for manipulating the form
 * specifically for the remove state.
 *  @name Editor#initRemove
 *  @event
 *  @param {event} e jQuery event object
 *  @param {array} trs Array of the TR elements for the removed to be deleted
 *  @param {array} data Array of the data source array / objects for the rows to
 *    be deleted. This is in the same index order as the TR nodes in the second
 *    parameter.
 */

/**
 * Pre-remove row event, fired just before DataTables calls the fnDeleteRow method
 * to delete a DataTables row.
 *  @name Editor#preRemove
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 */

/**
 * Row removed event, fired when a row has been removed in the DataTable by a form
 * submission. This is called just after the fnDeleteRow call to the DataTable.
 *  @name Editor#remove
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 */

/**
 * As per the `postRemove` event - included for naming consistency.
 *  @name Editor#postRemove
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 */

/**
 * Set data event, fired when the data is gathered from the form to be used
 * to update the DataTable. This is a "global" version of `preCreate`, `preEdit`
 * and `preRemove` and can be used to manipulate the data that will be added
 * to the DataTable for all three actions
 *  @name Editor#setData
 *  @event
 *  @param {event} e jQuery event object
 *  @param {object} json The JSON object returned from the server
 *  @param {object} data The data that will be used to update the DataTable
 *  @param {string} action The action being performed by the form - 'create',
 *    'edit' or 'remove'.
 */

/**
 * Initialisation of the Editor instance has been completed.
 *  @name Editor#initComplete
 *  @event
 *  @param {event} e jQuery event object
 */


return Editor;
}));


/*! Buttons for DataTables 1.5.1
 * ©2016-2017 SpryMedia Ltd - datatables.net/license
 */

(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


// Used for namespacing events added to the document by each instance, so they
// can be removed on destroy
var _instCounter = 0;

// Button namespacing counter for namespacing events on individual buttons
var _buttonCounter = 0;

var _dtButtons = DataTable.ext.buttons;

/**
 * [Buttons description]
 * @param {[type]}
 * @param {[type]}
 */
var Buttons = function( dt, config )
{
	// If there is no config set it to an empty object
	if ( typeof( config ) === 'undefined' ) {
		config = {};	
	}
	
	// Allow a boolean true for defaults
	if ( config === true ) {
		config = {};
	}

	// For easy configuration of buttons an array can be given
	if ( $.isArray( config ) ) {
		config = { buttons: config };
	}

	this.c = $.extend( true, {}, Buttons.defaults, config );

	// Don't want a deep copy for the buttons
	if ( config.buttons ) {
		this.c.buttons = config.buttons;
	}

	this.s = {
		dt: new DataTable.Api( dt ),
		buttons: [],
		listenKeys: '',
		namespace: 'dtb'+(_instCounter++)
	};

	this.dom = {
		container: $('<'+this.c.dom.container.tag+'/>')
			.addClass( this.c.dom.container.className )
	};

	this._constructor();
};


$.extend( Buttons.prototype, {
	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Public methods
	 */

	/**
	 * Get the action of a button
	 * @param  {int|string} Button index
	 * @return {function}
	 *//**
	 * Set the action of a button
	 * @param  {node} node Button element
	 * @param  {function} action Function to set
	 * @return {Buttons} Self for chaining
	 */
	action: function ( node, action )
	{
		var button = this._nodeToButton( node );

		if ( action === undefined ) {
			return button.conf.action;
		}

		button.conf.action = action;

		return this;
	},

	/**
	 * Add an active class to the button to make to look active or get current
	 * active state.
	 * @param  {node} node Button element
	 * @param  {boolean} [flag] Enable / disable flag
	 * @return {Buttons} Self for chaining or boolean for getter
	 */
	active: function ( node, flag ) {
		var button = this._nodeToButton( node );
		var klass = this.c.dom.button.active;
		var jqNode = $(button.node);

		if ( flag === undefined ) {
			return jqNode.hasClass( klass );
		}

		jqNode.toggleClass( klass, flag === undefined ? true : flag );

		return this;
	},

	/**
	 * Add a new button
	 * @param {object} config Button configuration object, base string name or function
	 * @param {int|string} [idx] Button index for where to insert the button
	 * @return {Buttons} Self for chaining
	 */
	add: function ( config, idx )
	{
		var buttons = this.s.buttons;

		if ( typeof idx === 'string' ) {
			var split = idx.split('-');
			var base = this.s;

			for ( var i=0, ien=split.length-1 ; i<ien ; i++ ) {
				base = base.buttons[ split[i]*1 ];
			}

			buttons = base.buttons;
			idx = split[ split.length-1 ]*1;
		}

		this._expandButton( buttons, config, false, idx );
		this._draw();

		return this;
	},

	/**
	 * Get the container node for the buttons
	 * @return {jQuery} Buttons node
	 */
	container: function ()
	{
		return this.dom.container;
	},

	/**
	 * Disable a button
	 * @param  {node} node Button node
	 * @return {Buttons} Self for chaining
	 */
	disable: function ( node ) {
		var button = this._nodeToButton( node );

		$(button.node).addClass( this.c.dom.button.disabled );

		return this;
	},

	/**
	 * Destroy the instance, cleaning up event handlers and removing DOM
	 * elements
	 * @return {Buttons} Self for chaining
	 */
	destroy: function ()
	{
		// Key event listener
		$('body').off( 'keyup.'+this.s.namespace );

		// Individual button destroy (so they can remove their own events if
		// needed). Take a copy as the array is modified by `remove`
		var buttons = this.s.buttons.slice();
		var i, ien;
		
		for ( i=0, ien=buttons.length ; i<ien ; i++ ) {
			this.remove( buttons[i].node );
		}

		// Container
		this.dom.container.remove();

		// Remove from the settings object collection
		var buttonInsts = this.s.dt.settings()[0];

		for ( i=0, ien=buttonInsts.length ; i<ien ; i++ ) {
			if ( buttonInsts.inst === this ) {
				buttonInsts.splice( i, 1 );
				break;
			}
		}

		return this;
	},

	/**
	 * Enable / disable a button
	 * @param  {node} node Button node
	 * @param  {boolean} [flag=true] Enable / disable flag
	 * @return {Buttons} Self for chaining
	 */
	enable: function ( node, flag )
	{
		if ( flag === false ) {
			return this.disable( node );
		}

		var button = this._nodeToButton( node );
		$(button.node).removeClass( this.c.dom.button.disabled );

		return this;
	},

	/**
	 * Get the instance name for the button set selector
	 * @return {string} Instance name
	 */
	name: function ()
	{
		return this.c.name;
	},

	/**
	 * Get a button's node
	 * @param  {node} node Button node
	 * @return {jQuery} Button element
	 */
	node: function ( node )
	{
		var button = this._nodeToButton( node );
		return $(button.node);
	},

	/**
	 * Set / get a processing class on the selected button
	 * @param  {boolean} flag true to add, false to remove, undefined to get
	 * @return {boolean|Buttons} Getter value or this if a setter.
	 */
	processing: function ( node, flag )
	{
		var button = this._nodeToButton( node );

		if ( flag === undefined ) {
			return $(button.node).hasClass( 'processing' );
		}

		$(button.node).toggleClass( 'processing', flag );

		return this;
	},

	/**
	 * Remove a button.
	 * @param  {node} node Button node
	 * @return {Buttons} Self for chaining
	 */
	remove: function ( node )
	{
		var button = this._nodeToButton( node );
		var host = this._nodeToHost( node );
		var dt = this.s.dt;

		// Remove any child buttons first
		if ( button.buttons.length ) {
			for ( var i=button.buttons.length-1 ; i>=0 ; i-- ) {
				this.remove( button.buttons[i].node );
			}
		}

		// Allow the button to remove event handlers, etc
		if ( button.conf.destroy ) {
			button.conf.destroy.call( dt.button(node), dt, $(node), button.conf );
		}

		this._removeKey( button.conf );

		$(button.node).remove();

		var idx = $.inArray( button, host );
		host.splice( idx, 1 );

		return this;
	},

	/**
	 * Get the text for a button
	 * @param  {int|string} node Button index
	 * @return {string} Button text
	 *//**
	 * Set the text for a button
	 * @param  {int|string|function} node Button index
	 * @param  {string} label Text
	 * @return {Buttons} Self for chaining
	 */
	text: function ( node, label )
	{
		var button = this._nodeToButton( node );
		var buttonLiner = this.c.dom.collection.buttonLiner;
		var linerTag = button.inCollection && buttonLiner && buttonLiner.tag ?
			buttonLiner.tag :
			this.c.dom.buttonLiner.tag;
		var dt = this.s.dt;
		var jqNode = $(button.node);
		var text = function ( opt ) {
			return typeof opt === 'function' ?
				opt( dt, jqNode, button.conf ) :
				opt;
		};

		if ( label === undefined ) {
			return text( button.conf.text );
		}

		button.conf.text = label;

		if ( linerTag ) {
			jqNode.children( linerTag ).html( text(label) );
		}
		else {
			jqNode.html( text(label) );
		}

		return this;
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Constructor
	 */

	/**
	 * Buttons constructor
	 * @private
	 */
	_constructor: function ()
	{
		var that = this;
		var dt = this.s.dt;
		var dtSettings = dt.settings()[0];
		var buttons =  this.c.buttons;

		if ( ! dtSettings._buttons ) {
			dtSettings._buttons = [];
		}

		dtSettings._buttons.push( {
			inst: this,
			name: this.c.name
		} );

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			this.add( buttons[i] );
		}

		dt.on( 'destroy', function () {
			that.destroy();
		} );

		// Global key event binding to listen for button keys
		$('body').on( 'keyup.'+this.s.namespace, function ( e ) {
			if ( ! document.activeElement || document.activeElement === document.body ) {
				// SUse a string of characters for fast lookup of if we need to
				// handle this
				var character = String.fromCharCode(e.keyCode).toLowerCase();

				if ( that.s.listenKeys.toLowerCase().indexOf( character ) !== -1 ) {
					that._keypress( character, e );
				}
			}
		} );
	},


	/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * Private methods
	 */

	/**
	 * Add a new button to the key press listener
	 * @param {object} conf Resolved button configuration object
	 * @private
	 */
	_addKey: function ( conf )
	{
		if ( conf.key ) {
			this.s.listenKeys += $.isPlainObject( conf.key ) ?
				conf.key.key :
				conf.key;
		}
	},

	/**
	 * Insert the buttons into the container. Call without parameters!
	 * @param  {node} [container] Recursive only - Insert point
	 * @param  {array} [buttons] Recursive only - Buttons array
	 * @private
	 */
	_draw: function ( container, buttons )
	{
		if ( ! container ) {
			container = this.dom.container;
			buttons = this.s.buttons;
		}

		container.children().detach();

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			container.append( buttons[i].inserter );
			container.append( ' ' );

			if ( buttons[i].buttons && buttons[i].buttons.length ) {
				this._draw( buttons[i].collection, buttons[i].buttons );
			}
		}
	},

	/**
	 * Create buttons from an array of buttons
	 * @param  {array} attachTo Buttons array to attach to
	 * @param  {object} button Button definition
	 * @param  {boolean} inCollection true if the button is in a collection
	 * @private
	 */
	_expandButton: function ( attachTo, button, inCollection, attachPoint )
	{
		var dt = this.s.dt;
		var buttonCounter = 0;
		var buttons = ! $.isArray( button ) ?
			[ button ] :
			button;

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			var conf = this._resolveExtends( buttons[i] );

			if ( ! conf ) {
				continue;
			}

			// If the configuration is an array, then expand the buttons at this
			// point
			if ( $.isArray( conf ) ) {
				this._expandButton( attachTo, conf, inCollection, attachPoint );
				continue;
			}

			var built = this._buildButton( conf, inCollection );
			if ( ! built ) {
				continue;
			}

			if ( attachPoint !== undefined ) {
				attachTo.splice( attachPoint, 0, built );
				attachPoint++;
			}
			else {
				attachTo.push( built );
			}

			if ( built.conf.buttons ) {
				var collectionDom = this.c.dom.collection;
				built.collection = $('<'+collectionDom.tag+'/>')
					.addClass( collectionDom.className )
					.attr( 'role', 'menu') ;
				built.conf._collection = built.collection;

				this._expandButton( built.buttons, built.conf.buttons, true, attachPoint );
			}

			// init call is made here, rather than buildButton as it needs to
			// be selectable, and for that it needs to be in the buttons array
			if ( conf.init ) {
				conf.init.call( dt.button( built.node ), dt, $(built.node), conf );
			}

			buttonCounter++;
		}
	},

	/**
	 * Create an individual button
	 * @param  {object} config            Resolved button configuration
	 * @param  {boolean} inCollection `true` if a collection button
	 * @return {jQuery} Created button node (jQuery)
	 * @private
	 */
	_buildButton: function ( config, inCollection )
	{
		var buttonDom = this.c.dom.button;
		var linerDom = this.c.dom.buttonLiner;
		var collectionDom = this.c.dom.collection;
		var dt = this.s.dt;
		var text = function ( opt ) {
			return typeof opt === 'function' ?
				opt( dt, button, config ) :
				opt;
		};

		if ( inCollection && collectionDom.button ) {
			buttonDom = collectionDom.button;
		}

		if ( inCollection && collectionDom.buttonLiner ) {
			linerDom = collectionDom.buttonLiner;
		}

		// Make sure that the button is available based on whatever requirements
		// it has. For example, Flash buttons require Flash
		if ( config.available && ! config.available( dt, config ) ) {
			return false;
		}

		var action = function ( e, dt, button, config ) {
			config.action.call( dt.button( button ), e, dt, button, config );

			$(dt.table().node()).triggerHandler( 'buttons-action.dt', [
				dt.button( button ), dt, button, config 
			] );
		};

		var button = $('<'+buttonDom.tag+'/>')
			.addClass( buttonDom.className )
			.attr( 'tabindex', this.s.dt.settings()[0].iTabIndex )
			.attr( 'aria-controls', this.s.dt.table().node().id )
			.on( 'click.dtb', function (e) {
				e.preventDefault();

				if ( ! button.hasClass( buttonDom.disabled ) && config.action ) {
					action( e, dt, button, config );
				}

				button.blur();
			} )
			.on( 'keyup.dtb', function (e) {
				if ( e.keyCode === 13 ) {
					if ( ! button.hasClass( buttonDom.disabled ) && config.action ) {
						action( e, dt, button, config );
					}
				}
			} );

		// Make `a` tags act like a link
		if ( buttonDom.tag.toLowerCase() === 'a' ) {
			button.attr( 'href', '#' );
		}

		if ( linerDom.tag ) {
			var liner = $('<'+linerDom.tag+'/>')
				.html( text( config.text ) )
				.addClass( linerDom.className );

			if ( linerDom.tag.toLowerCase() === 'a' ) {
				liner.attr( 'href', '#' );
			}

			button.append( liner );
		}
		else {
			button.html( text( config.text ) );
		}

		if ( config.enabled === false ) {
			button.addClass( buttonDom.disabled );
		}

		if ( config.className ) {
			button.addClass( config.className );
		}

		if ( config.titleAttr ) {
			button.attr( 'title', text( config.titleAttr ) );
		}

		if ( config.attr ) {
			button.attr( config.attr );
		}

		if ( ! config.namespace ) {
			config.namespace = '.dt-button-'+(_buttonCounter++);
		}

		var buttonContainer = this.c.dom.buttonContainer;
		var inserter;
		if ( buttonContainer && buttonContainer.tag ) {
			inserter = $('<'+buttonContainer.tag+'/>')
				.addClass( buttonContainer.className )
				.append( button );
		}
		else {
			inserter = button;
		}

		this._addKey( config );

		return {
			conf:         config,
			node:         button.get(0),
			inserter:     inserter,
			buttons:      [],
			inCollection: inCollection,
			collection:   null
		};
	},

	/**
	 * Get the button object from a node (recursive)
	 * @param  {node} node Button node
	 * @param  {array} [buttons] Button array, uses base if not defined
	 * @return {object} Button object
	 * @private
	 */
	_nodeToButton: function ( node, buttons )
	{
		if ( ! buttons ) {
			buttons = this.s.buttons;
		}

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			if ( buttons[i].node === node ) {
				return buttons[i];
			}

			if ( buttons[i].buttons.length ) {
				var ret = this._nodeToButton( node, buttons[i].buttons );

				if ( ret ) {
					return ret;
				}
			}
		}
	},

	/**
	 * Get container array for a button from a button node (recursive)
	 * @param  {node} node Button node
	 * @param  {array} [buttons] Button array, uses base if not defined
	 * @return {array} Button's host array
	 * @private
	 */
	_nodeToHost: function ( node, buttons )
	{
		if ( ! buttons ) {
			buttons = this.s.buttons;
		}

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			if ( buttons[i].node === node ) {
				return buttons;
			}

			if ( buttons[i].buttons.length ) {
				var ret = this._nodeToHost( node, buttons[i].buttons );

				if ( ret ) {
					return ret;
				}
			}
		}
	},

	/**
	 * Handle a key press - determine if any button's key configured matches
	 * what was typed and trigger the action if so.
	 * @param  {string} character The character pressed
	 * @param  {object} e Key event that triggered this call
	 * @private
	 */
	_keypress: function ( character, e )
	{
		// Check if this button press already activated on another instance of Buttons
		if ( e._buttonsHandled ) {
			return;
		}

		var run = function ( conf, node ) {
			if ( ! conf.key ) {
				return;
			}

			if ( conf.key === character ) {
				e._buttonsHandled = true;
				$(node).click();
			}
			else if ( $.isPlainObject( conf.key ) ) {
				if ( conf.key.key !== character ) {
					return;
				}

				if ( conf.key.shiftKey && ! e.shiftKey ) {
					return;
				}

				if ( conf.key.altKey && ! e.altKey ) {
					return;
				}

				if ( conf.key.ctrlKey && ! e.ctrlKey ) {
					return;
				}

				if ( conf.key.metaKey && ! e.metaKey ) {
					return;
				}

				// Made it this far - it is good
				e._buttonsHandled = true;
				$(node).click();
			}
		};

		var recurse = function ( a ) {
			for ( var i=0, ien=a.length ; i<ien ; i++ ) {
				run( a[i].conf, a[i].node );

				if ( a[i].buttons.length ) {
					recurse( a[i].buttons );
				}
			}
		};

		recurse( this.s.buttons );
	},

	/**
	 * Remove a key from the key listener for this instance (to be used when a
	 * button is removed)
	 * @param  {object} conf Button configuration
	 * @private
	 */
	_removeKey: function ( conf )
	{
		if ( conf.key ) {
			var character = $.isPlainObject( conf.key ) ?
				conf.key.key :
				conf.key;

			// Remove only one character, as multiple buttons could have the
			// same listening key
			var a = this.s.listenKeys.split('');
			var idx = $.inArray( character, a );
			a.splice( idx, 1 );
			this.s.listenKeys = a.join('');
		}
	},

	/**
	 * Resolve a button configuration
	 * @param  {string|function|object} conf Button config to resolve
	 * @return {object} Button configuration
	 * @private
	 */
	_resolveExtends: function ( conf )
	{
		var dt = this.s.dt;
		var i, ien;
		var toConfObject = function ( base ) {
			var loop = 0;

			// Loop until we have resolved to a button configuration, or an
			// array of button configurations (which will be iterated
			// separately)
			while ( ! $.isPlainObject(base) && ! $.isArray(base) ) {
				if ( base === undefined ) {
					return;
				}

				if ( typeof base === 'function' ) {
					base = base( dt, conf );

					if ( ! base ) {
						return false;
					}
				}
				else if ( typeof base === 'string' ) {
					if ( ! _dtButtons[ base ] ) {
						throw 'Unknown button type: '+base;
					}

					base = _dtButtons[ base ];
				}

				loop++;
				if ( loop > 30 ) {
					// Protect against misconfiguration killing the browser
					throw 'Buttons: Too many iterations';
				}
			}

			return $.isArray( base ) ?
				base :
				$.extend( {}, base );
		};

		conf = toConfObject( conf );

		while ( conf && conf.extend ) {
			// Use `toConfObject` in case the button definition being extended
			// is itself a string or a function
			if ( ! _dtButtons[ conf.extend ] ) {
				throw 'Cannot extend unknown button type: '+conf.extend;
			}

			var objArray = toConfObject( _dtButtons[ conf.extend ] );
			if ( $.isArray( objArray ) ) {
				return objArray;
			}
			else if ( ! objArray ) {
				// This is a little brutal as it might be possible to have a
				// valid button without the extend, but if there is no extend
				// then the host button would be acting in an undefined state
				return false;
			}

			// Stash the current class name
			var originalClassName = objArray.className;

			conf = $.extend( {}, objArray, conf );

			// The extend will have overwritten the original class name if the
			// `conf` object also assigned a class, but we want to concatenate
			// them so they are list that is combined from all extended buttons
			if ( originalClassName && conf.className !== originalClassName ) {
				conf.className = originalClassName+' '+conf.className;
			}

			// Buttons to be added to a collection  -gives the ability to define
			// if buttons should be added to the start or end of a collection
			var postfixButtons = conf.postfixButtons;
			if ( postfixButtons ) {
				if ( ! conf.buttons ) {
					conf.buttons = [];
				}

				for ( i=0, ien=postfixButtons.length ; i<ien ; i++ ) {
					conf.buttons.push( postfixButtons[i] );
				}

				conf.postfixButtons = null;
			}

			var prefixButtons = conf.prefixButtons;
			if ( prefixButtons ) {
				if ( ! conf.buttons ) {
					conf.buttons = [];
				}

				for ( i=0, ien=prefixButtons.length ; i<ien ; i++ ) {
					conf.buttons.splice( i, 0, prefixButtons[i] );
				}

				conf.prefixButtons = null;
			}

			// Although we want the `conf` object to overwrite almost all of
			// the properties of the object being extended, the `extend`
			// property should come from the object being extended
			conf.extend = objArray.extend;
		}

		return conf;
	}
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Statics
 */

/**
 * Show / hide a background layer behind a collection
 * @param  {boolean} Flag to indicate if the background should be shown or
 *   hidden 
 * @param  {string} Class to assign to the background
 * @static
 */
Buttons.background = function ( show, className, fade ) {
	if ( fade === undefined ) {
		fade = 400;
	}

	if ( show ) {
		$('<div/>')
			.addClass( className )
			.css( 'display', 'none' )
			.appendTo( 'body' )
			.fadeIn( fade );
	}
	else {
		$('body > div.'+className)
			.fadeOut( fade, function () {
				$(this)
					.removeClass( className )
					.remove();
			} );
	}
};

/**
 * Instance selector - select Buttons instances based on an instance selector
 * value from the buttons assigned to a DataTable. This is only useful if
 * multiple instances are attached to a DataTable.
 * @param  {string|int|array} Instance selector - see `instance-selector`
 *   documentation on the DataTables site
 * @param  {array} Button instance array that was attached to the DataTables
 *   settings object
 * @return {array} Buttons instances
 * @static
 */
Buttons.instanceSelector = function ( group, buttons )
{
	if ( ! group ) {
		return $.map( buttons, function ( v ) {
			return v.inst;
		} );
	}

	var ret = [];
	var names = $.map( buttons, function ( v ) {
		return v.name;
	} );

	// Flatten the group selector into an array of single options
	var process = function ( input ) {
		if ( $.isArray( input ) ) {
			for ( var i=0, ien=input.length ; i<ien ; i++ ) {
				process( input[i] );
			}
			return;
		}

		if ( typeof input === 'string' ) {
			if ( input.indexOf( ',' ) !== -1 ) {
				// String selector, list of names
				process( input.split(',') );
			}
			else {
				// String selector individual name
				var idx = $.inArray( $.trim(input), names );

				if ( idx !== -1 ) {
					ret.push( buttons[ idx ].inst );
				}
			}
		}
		else if ( typeof input === 'number' ) {
			// Index selector
			ret.push( buttons[ input ].inst );
		}
	};
	
	process( group );

	return ret;
};

/**
 * Button selector - select one or more buttons from a selector input so some
 * operation can be performed on them.
 * @param  {array} Button instances array that the selector should operate on
 * @param  {string|int|node|jQuery|array} Button selector - see
 *   `button-selector` documentation on the DataTables site
 * @return {array} Array of objects containing `inst` and `idx` properties of
 *   the selected buttons so you know which instance each button belongs to.
 * @static
 */
Buttons.buttonSelector = function ( insts, selector )
{
	var ret = [];
	var nodeBuilder = function ( a, buttons, baseIdx ) {
		var button;
		var idx;

		for ( var i=0, ien=buttons.length ; i<ien ; i++ ) {
			button = buttons[i];

			if ( button ) {
				idx = baseIdx !== undefined ?
					baseIdx+i :
					i+'';

				a.push( {
					node: button.node,
					name: button.conf.name,
					idx:  idx
				} );

				if ( button.buttons ) {
					nodeBuilder( a, button.buttons, idx+'-' );
				}
			}
		}
	};

	var run = function ( selector, inst ) {
		var i, ien;
		var buttons = [];
		nodeBuilder( buttons, inst.s.buttons );

		var nodes = $.map( buttons, function (v) {
			return v.node;
		} );

		if ( $.isArray( selector ) || selector instanceof $ ) {
			for ( i=0, ien=selector.length ; i<ien ; i++ ) {
				run( selector[i], inst );
			}
			return;
		}

		if ( selector === null || selector === undefined || selector === '*' ) {
			// Select all
			for ( i=0, ien=buttons.length ; i<ien ; i++ ) {
				ret.push( {
					inst: inst,
					node: buttons[i].node
				} );
			}
		}
		else if ( typeof selector === 'number' ) {
			// Main button index selector
			ret.push( {
				inst: inst,
				node: inst.s.buttons[ selector ].node
			} );
		}
		else if ( typeof selector === 'string' ) {
			if ( selector.indexOf( ',' ) !== -1 ) {
				// Split
				var a = selector.split(',');

				for ( i=0, ien=a.length ; i<ien ; i++ ) {
					run( $.trim(a[i]), inst );
				}
			}
			else if ( selector.match( /^\d+(\-\d+)*$/ ) ) {
				// Sub-button index selector
				var indexes = $.map( buttons, function (v) {
					return v.idx;
				} );

				ret.push( {
					inst: inst,
					node: buttons[ $.inArray( selector, indexes ) ].node
				} );
			}
			else if ( selector.indexOf( ':name' ) !== -1 ) {
				// Button name selector
				var name = selector.replace( ':name', '' );

				for ( i=0, ien=buttons.length ; i<ien ; i++ ) {
					if ( buttons[i].name === name ) {
						ret.push( {
							inst: inst,
							node: buttons[i].node
						} );
					}
				}
			}
			else {
				// jQuery selector on the nodes
				$( nodes ).filter( selector ).each( function () {
					ret.push( {
						inst: inst,
						node: this
					} );
				} );
			}
		}
		else if ( typeof selector === 'object' && selector.nodeName ) {
			// Node selector
			var idx = $.inArray( selector, nodes );

			if ( idx !== -1 ) {
				ret.push( {
					inst: inst,
					node: nodes[ idx ]
				} );
			}
		}
	};


	for ( var i=0, ien=insts.length ; i<ien ; i++ ) {
		var inst = insts[i];

		run( selector, inst );
	}

	return ret;
};


/**
 * Buttons defaults. For full documentation, please refer to the docs/option
 * directory or the DataTables site.
 * @type {Object}
 * @static
 */
Buttons.defaults = {
	buttons: [ 'copy', 'excel', 'csv', 'pdf', 'print' ],
	name: 'main',
	tabIndex: 0,
	dom: {
		container: {
			tag: 'div',
			className: 'dt-buttons'
		},
		collection: {
			tag: 'div',
			className: 'dt-button-collection'
		},
		button: {
			tag: 'button',
			className: 'dt-button',
			active: 'active',
			disabled: 'disabled'
		},
		buttonLiner: {
			tag: 'span',
			className: ''
		}
	}
};

/**
 * Version information
 * @type {string}
 * @static
 */
Buttons.version = '1.5.1';


$.extend( _dtButtons, {
	collection: {
		text: function ( dt ) {
			return dt.i18n( 'buttons.collection', 'Collection' );
		},
		className: 'buttons-collection',
		action: function ( e, dt, button, config ) {
			var host = button;
			var collectionParent = $(button).parents('div.dt-button-collection');
			var hostPosition = host.position();
			var tableContainer = $( dt.table().container() );
			var multiLevel = false;
			var insertPoint = host;

			// Remove any old collection
			if ( collectionParent.length ) {
				multiLevel = $('.dt-button-collection').position();
				insertPoint = collectionParent;
				$('body').trigger( 'click.dtb-collection' );
			}

			config._collection
				.addClass( config.collectionLayout )
				.css( 'display', 'none' )
				.insertAfter( insertPoint )
				.fadeIn( config.fade );
			

			var position = config._collection.css( 'position' );

			if ( multiLevel && position === 'absolute' ) {
				config._collection.css( {
					top: multiLevel.top,
					left: multiLevel.left
				} );
			}
			else if ( position === 'absolute' ) {
				config._collection.css( {
					top: hostPosition.top + host.outerHeight(),
					left: hostPosition.left
				} );

				// calculate overflow when positioned beneath
				var tableBottom = tableContainer.offset().top + tableContainer.height();
				var listBottom = hostPosition.top + host.outerHeight() + config._collection.outerHeight();
				var bottomOverflow = listBottom - tableBottom;
				
				// calculate overflow when positioned above
				var listTop = hostPosition.top - config._collection.outerHeight();
				var tableTop = tableContainer.offset().top;
				var topOverflow = tableTop - listTop;
				
				// if bottom overflow is larger, move to the top because it fits better
				if (bottomOverflow > topOverflow) {
					config._collection.css( 'top', hostPosition.top - config._collection.outerHeight() - 5);
				}

				var listRight = hostPosition.left + config._collection.outerWidth();
				var tableRight = tableContainer.offset().left + tableContainer.width();
				if ( listRight > tableRight ) {
					config._collection.css( 'left', hostPosition.left - ( listRight - tableRight ) );
				}
			}
			else {
				// Fix position - centre on screen
				var top = config._collection.height() / 2;
				if ( top > $(window).height() / 2 ) {
					top = $(window).height() / 2;
				}

				config._collection.css( 'marginTop', top*-1 );
			}

			if ( config.background ) {
				Buttons.background( true, config.backgroundClassName, config.fade );
			}

			// Need to break the 'thread' for the collection button being
			// activated by a click - it would also trigger this event
			setTimeout( function () {
				// This is bonkers, but if we don't have a click listener on the
				// background element, iOS Safari will ignore the body click
				// listener below. An empty function here is all that is
				// required to make it work...
				$('div.dt-button-background').on( 'click.dtb-collection', function () {} );

				$('body').on( 'click.dtb-collection', function (e) {
					// andSelf is deprecated in jQ1.8, but we want 1.7 compat
					var back = $.fn.addBack ? 'addBack' : 'andSelf';

					if ( ! $(e.target).parents()[back]().filter( config._collection ).length ) {
						config._collection
							.fadeOut( config.fade, function () {
								config._collection.detach();
							} );

						$('div.dt-button-background').off( 'click.dtb-collection' );
						Buttons.background( false, config.backgroundClassName, config.fade );

						$('body').off( 'click.dtb-collection' );
						dt.off( 'buttons-action.b-internal' );
					}
				} );
			}, 10 );

			if ( config.autoClose ) {
				dt.on( 'buttons-action.b-internal', function () {
					$('div.dt-button-background').click();
				} );
			}
		},
		background: true,
		collectionLayout: '',
		backgroundClassName: 'dt-button-background',
		autoClose: false,
		fade: 400,
		attr: {
			'aria-haspopup': true
		}
	},
	copy: function ( dt, conf ) {
		if ( _dtButtons.copyHtml5 ) {
			return 'copyHtml5';
		}
		if ( _dtButtons.copyFlash && _dtButtons.copyFlash.available( dt, conf ) ) {
			return 'copyFlash';
		}
	},
	csv: function ( dt, conf ) {
		// Common option that will use the HTML5 or Flash export buttons
		if ( _dtButtons.csvHtml5 && _dtButtons.csvHtml5.available( dt, conf ) ) {
			return 'csvHtml5';
		}
		if ( _dtButtons.csvFlash && _dtButtons.csvFlash.available( dt, conf ) ) {
			return 'csvFlash';
		}
	},
	excel: function ( dt, conf ) {
		// Common option that will use the HTML5 or Flash export buttons
		if ( _dtButtons.excelHtml5 && _dtButtons.excelHtml5.available( dt, conf ) ) {
			return 'excelHtml5';
		}
		if ( _dtButtons.excelFlash && _dtButtons.excelFlash.available( dt, conf ) ) {
			return 'excelFlash';
		}
	},
	pdf: function ( dt, conf ) {
		// Common option that will use the HTML5 or Flash export buttons
		if ( _dtButtons.pdfHtml5 && _dtButtons.pdfHtml5.available( dt, conf ) ) {
			return 'pdfHtml5';
		}
		if ( _dtButtons.pdfFlash && _dtButtons.pdfFlash.available( dt, conf ) ) {
			return 'pdfFlash';
		}
	},
	pageLength: function ( dt ) {
		var lengthMenu = dt.settings()[0].aLengthMenu;
		var vals = $.isArray( lengthMenu[0] ) ? lengthMenu[0] : lengthMenu;
		var lang = $.isArray( lengthMenu[0] ) ? lengthMenu[1] : lengthMenu;
		var text = function ( dt ) {
			return dt.i18n( 'buttons.pageLength', {
				"-1": 'Show all rows',
				_:    'Show %d rows'
			}, dt.page.len() );
		};

		return {
			extend: 'collection',
			text: text,
			className: 'buttons-page-length',
			autoClose: true,
			buttons: $.map( vals, function ( val, i ) {
				return {
					text: lang[i],
					className: 'button-page-length',
					action: function ( e, dt ) {
						dt.page.len( val ).draw();
					},
					init: function ( dt, node, conf ) {
						var that = this;
						var fn = function () {
							that.active( dt.page.len() === val );
						};

						dt.on( 'length.dt'+conf.namespace, fn );
						fn();
					},
					destroy: function ( dt, node, conf ) {
						dt.off( 'length.dt'+conf.namespace );
					}
				};
			} ),
			init: function ( dt, node, conf ) {
				var that = this;
				dt.on( 'length.dt'+conf.namespace, function () {
					that.text( text( dt ) );
				} );
			},
			destroy: function ( dt, node, conf ) {
				dt.off( 'length.dt'+conf.namespace );
			}
		};
	}
} );


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables API
 *
 * For complete documentation, please refer to the docs/api directory or the
 * DataTables site
 */

// Buttons group and individual button selector
DataTable.Api.register( 'buttons()', function ( group, selector ) {
	// Argument shifting
	if ( selector === undefined ) {
		selector = group;
		group = undefined;
	}

	this.selector.buttonGroup = group;

	var res = this.iterator( true, 'table', function ( ctx ) {
		if ( ctx._buttons ) {
			return Buttons.buttonSelector(
				Buttons.instanceSelector( group, ctx._buttons ),
				selector
			);
		}
	}, true );

	res._groupSelector = group;
	return res;
} );

// Individual button selector
DataTable.Api.register( 'button()', function ( group, selector ) {
	// just run buttons() and truncate
	var buttons = this.buttons( group, selector );

	if ( buttons.length > 1 ) {
		buttons.splice( 1, buttons.length );
	}

	return buttons;
} );

// Active buttons
DataTable.Api.registerPlural( 'buttons().active()', 'button().active()', function ( flag ) {
	if ( flag === undefined ) {
		return this.map( function ( set ) {
			return set.inst.active( set.node );
		} );
	}

	return this.each( function ( set ) {
		set.inst.active( set.node, flag );
	} );
} );

// Get / set button action
DataTable.Api.registerPlural( 'buttons().action()', 'button().action()', function ( action ) {
	if ( action === undefined ) {
		return this.map( function ( set ) {
			return set.inst.action( set.node );
		} );
	}

	return this.each( function ( set ) {
		set.inst.action( set.node, action );
	} );
} );

// Enable / disable buttons
DataTable.Api.register( ['buttons().enable()', 'button().enable()'], function ( flag ) {
	return this.each( function ( set ) {
		set.inst.enable( set.node, flag );
	} );
} );

// Disable buttons
DataTable.Api.register( ['buttons().disable()', 'button().disable()'], function () {
	return this.each( function ( set ) {
		set.inst.disable( set.node );
	} );
} );

// Get button nodes
DataTable.Api.registerPlural( 'buttons().nodes()', 'button().node()', function () {
	var jq = $();

	// jQuery will automatically reduce duplicates to a single entry
	$( this.each( function ( set ) {
		jq = jq.add( set.inst.node( set.node ) );
	} ) );

	return jq;
} );

// Get / set button processing state
DataTable.Api.registerPlural( 'buttons().processing()', 'button().processing()', function ( flag ) {
	if ( flag === undefined ) {
		return this.map( function ( set ) {
			return set.inst.processing( set.node );
		} );
	}

	return this.each( function ( set ) {
		set.inst.processing( set.node, flag );
	} );
} );

// Get / set button text (i.e. the button labels)
DataTable.Api.registerPlural( 'buttons().text()', 'button().text()', function ( label ) {
	if ( label === undefined ) {
		return this.map( function ( set ) {
			return set.inst.text( set.node );
		} );
	}

	return this.each( function ( set ) {
		set.inst.text( set.node, label );
	} );
} );

// Trigger a button's action
DataTable.Api.registerPlural( 'buttons().trigger()', 'button().trigger()', function () {
	return this.each( function ( set ) {
		set.inst.node( set.node ).trigger( 'click' );
	} );
} );

// Get the container elements
DataTable.Api.registerPlural( 'buttons().containers()', 'buttons().container()', function () {
	var jq = $();
	var groupSelector = this._groupSelector;

	// We need to use the group selector directly, since if there are no buttons
	// the result set will be empty
	this.iterator( true, 'table', function ( ctx ) {
		if ( ctx._buttons ) {
			var insts = Buttons.instanceSelector( groupSelector, ctx._buttons );

			for ( var i=0, ien=insts.length ; i<ien ; i++ ) {
				jq = jq.add( insts[i].container() );
			}
		}
	} );

	return jq;
} );

// Add a new button
DataTable.Api.register( 'button().add()', function ( idx, conf ) {
	var ctx = this.context;

	// Don't use `this` as it could be empty - select the instances directly
	if ( ctx.length ) {
		var inst = Buttons.instanceSelector( this._groupSelector, ctx[0]._buttons );

		if ( inst.length ) {
			inst[0].add( conf, idx );
		}
	}

	return this.button( this._groupSelector, idx );
} );

// Destroy the button sets selected
DataTable.Api.register( 'buttons().destroy()', function () {
	this.pluck( 'inst' ).unique().each( function ( inst ) {
		inst.destroy();
	} );

	return this;
} );

// Remove a button
DataTable.Api.registerPlural( 'buttons().remove()', 'buttons().remove()', function () {
	this.each( function ( set ) {
		set.inst.remove( set.node );
	} );

	return this;
} );

// Information box that can be used by buttons
var _infoTimer;
DataTable.Api.register( 'buttons.info()', function ( title, message, time ) {
	var that = this;

	if ( title === false ) {
		$('#datatables_buttons_info').fadeOut( function () {
			$(this).remove();
		} );
		clearTimeout( _infoTimer );
		_infoTimer = null;

		return this;
	}

	if ( _infoTimer ) {
		clearTimeout( _infoTimer );
	}

	if ( $('#datatables_buttons_info').length ) {
		$('#datatables_buttons_info').remove();
	}

	title = title ? '<h2>'+title+'</h2>' : '';

	$('<div id="datatables_buttons_info" class="dt-button-info"/>')
		.html( title )
		.append( $('<div/>')[ typeof message === 'string' ? 'html' : 'append' ]( message ) )
		.css( 'display', 'none' )
		.appendTo( 'body' )
		.fadeIn();

	if ( time !== undefined && time !== 0 ) {
		_infoTimer = setTimeout( function () {
			that.buttons.info( false );
		}, time );
	}

	return this;
} );

// Get data from the table for export - this is common to a number of plug-in
// buttons so it is included in the Buttons core library
DataTable.Api.register( 'buttons.exportData()', function ( options ) {
	if ( this.context.length ) {
		return _exportData( new DataTable.Api( this.context[0] ), options );
	}
} );

// Get information about the export that is common to many of the export data
// types (DRY)
DataTable.Api.register( 'buttons.exportInfo()', function ( conf ) {
	if ( ! conf ) {
		conf = {};
	}

	return {
		filename: _filename( conf ),
		title: _title( conf ),
		messageTop: _message(this, conf.message || conf.messageTop, 'top'),
		messageBottom: _message(this, conf.messageBottom, 'bottom')
	};
} );



/**
 * Get the file name for an exported file.
 *
 * @param {object}	config Button configuration
 * @param {boolean} incExtension Include the file name extension
 */
var _filename = function ( config )
{
	// Backwards compatibility
	var filename = config.filename === '*' && config.title !== '*' && config.title !== undefined && config.title !== null && config.title !== '' ?
		config.title :
		config.filename;

	if ( typeof filename === 'function' ) {
		filename = filename();
	}

	if ( filename === undefined || filename === null ) {
		return null;
	}

	if ( filename.indexOf( '*' ) !== -1 ) {
		filename = $.trim( filename.replace( '*', $('head > title').text() ) );
	}

	// Strip characters which the OS will object to
	filename = filename.replace(/[^a-zA-Z0-9_\u00A1-\uFFFF\.,\-_ !\(\)]/g, "");

	var extension = _stringOrFunction( config.extension );
	if ( ! extension ) {
		extension = '';
	}

	return filename + extension;
};

/**
 * Simply utility method to allow parameters to be given as a function
 *
 * @param {undefined|string|function} option Option
 * @return {null|string} Resolved value
 */
var _stringOrFunction = function ( option )
{
	if ( option === null || option === undefined ) {
		return null;
	}
	else if ( typeof option === 'function' ) {
		return option();
	}
	return option;
};

/**
 * Get the title for an exported file.
 *
 * @param {object} config	Button configuration
 */
var _title = function ( config )
{
	var title = _stringOrFunction( config.title );

	return title === null ?
		null : title.indexOf( '*' ) !== -1 ?
			title.replace( '*', $('head > title').text() || 'Exported data' ) :
			title;
};

var _message = function ( dt, option, position )
{
	var message = _stringOrFunction( option );
	if ( message === null ) {
		return null;
	}

	var caption = $('caption', dt.table().container()).eq(0);
	if ( message === '*' ) {
		var side = caption.css( 'caption-side' );
		if ( side !== position ) {
			return null;
		}

		return caption.length ?
			caption.text() :
			'';
	}

	return message;
};







var _exportTextarea = $('<textarea/>')[0];
var _exportData = function ( dt, inOpts )
{
	var config = $.extend( true, {}, {
		rows:           null,
		columns:        '',
		modifier:       {
			search: 'applied',
			order:  'applied'
		},
		orthogonal:     'display',
		stripHtml:      true,
		stripNewlines:  true,
		decodeEntities: true,
		trim:           true,
		format:         {
			header: function ( d ) {
				return strip( d );
			},
			footer: function ( d ) {
				return strip( d );
			},
			body: function ( d ) {
				return strip( d );
			}
		}
	}, inOpts );

	var strip = function ( str ) {
		if ( typeof str !== 'string' ) {
			return str;
		}

		// Always remove script tags
		str = str.replace( /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '' );

		if ( config.stripHtml ) {
			str = str.replace( /<[^>]*>/g, '' );
		}

		if ( config.trim ) {
			str = str.replace( /^\s+|\s+$/g, '' );
		}

		if ( config.stripNewlines ) {
			str = str.replace( /\n/g, ' ' );
		}

		if ( config.decodeEntities ) {
			_exportTextarea.innerHTML = str;
			str = _exportTextarea.value;
		}

		return str;
	};


	var header = dt.columns( config.columns ).indexes().map( function (idx) {
		var el = dt.column( idx ).header();
		return config.format.header( el.innerHTML, idx, el );
	} ).toArray();

	var footer = dt.table().footer() ?
		dt.columns( config.columns ).indexes().map( function (idx) {
			var el = dt.column( idx ).footer();
			return config.format.footer( el ? el.innerHTML : '', idx, el );
		} ).toArray() :
		null;
	
	// If Select is available on this table, and any rows are selected, limit the export
	// to the selected rows. If no rows are selected, all rows will be exported. Specify
	// a `selected` modifier to control directly.
	var modifier = $.extend( {}, config.modifier );
	if ( dt.select && typeof dt.select.info === 'function' && modifier.selected === undefined ) {
		if ( dt.rows( config.rows, $.extend( { selected: true }, modifier ) ).any() ) {
			$.extend( modifier, { selected: true } )
		}
	}

	var rowIndexes = dt.rows( config.rows, modifier ).indexes().toArray();
	var selectedCells = dt.cells( rowIndexes, config.columns );
	var cells = selectedCells
		.render( config.orthogonal )
		.toArray();
	var cellNodes = selectedCells
		.nodes()
		.toArray();

	var columns = header.length;
	var rows = columns > 0 ? cells.length / columns : 0;
	var body = [ rows ];
	var cellCounter = 0;

	for ( var i=0, ien=rows ; i<ien ; i++ ) {
		var row = [ columns ];

		for ( var j=0 ; j<columns ; j++ ) {
			row[j] = config.format.body( cells[ cellCounter ], i, j, cellNodes[ cellCounter ] );
			cellCounter++;
		}

		body[i] = row;
	}

	return {
		header: header,
		footer: footer,
		body:   body
	};
};


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables interface
 */

// Attach to DataTables objects for global access
$.fn.dataTable.Buttons = Buttons;
$.fn.DataTable.Buttons = Buttons;



// DataTables creation - check if the buttons have been defined for this table,
// they will have been if the `B` option was used in `dom`, otherwise we should
// create the buttons instance here so they can be inserted into the document
// using the API. Listen for `init` for compatibility with pre 1.10.10, but to
// be removed in future.
$(document).on( 'init.dt plugin-init.dt', function (e, settings) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	var opts = settings.oInit.buttons || DataTable.defaults.buttons;

	if ( opts && ! settings._buttons ) {
		new Buttons( settings, opts ).container();
	}
} );

// DataTables `dom` feature option
DataTable.ext.feature.push( {
	fnInit: function( settings ) {
		var api = new DataTable.Api( settings );
		var opts = api.init().buttons || DataTable.defaults.buttons;

		return new Buttons( api, opts ).container();
	},
	cFeature: "B"
} );


return Buttons;
}));


/*! Select for DataTables 1.2.4
 * 2015-2017 SpryMedia Ltd - datatables.net/license/mit
 */

/**
 * @summary     Select for DataTables
 * @description A collection of API methods, events and buttons for DataTables
 *   that provides selection options of the items in a DataTable
 * @version     1.2.4
 * @file        dataTables.select.js
 * @author      SpryMedia Ltd (www.sprymedia.co.uk)
 * @contact     datatables.net/forums
 * @copyright   Copyright 2015-2017 SpryMedia Ltd.
 *
 * This source file is free software, available under the following license:
 *   MIT license - http://datatables.net/license/mit
 *
 * This source file is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the license files for details.
 *
 * For details please refer to: http://www.datatables.net/extensions/select
 */
(function( factory ){
	if ( typeof define === 'function' && define.amd ) {
		// AMD
		define( ['jquery', 'datatables.net'], function ( $ ) {
			return factory( $, window, document );
		} );
	}
	else if ( typeof exports === 'object' ) {
		// CommonJS
		module.exports = function (root, $) {
			if ( ! root ) {
				root = window;
			}

			if ( ! $ || ! $.fn.dataTable ) {
				$ = require('datatables.net')(root, $).$;
			}

			return factory( $, root, root.document );
		};
	}
	else {
		// Browser
		factory( jQuery, window, document );
	}
}(function( $, window, document, undefined ) {
'use strict';
var DataTable = $.fn.dataTable;


// Version information for debugger
DataTable.select = {};

DataTable.select.version = '1.2.4';

DataTable.select.init = function ( dt ) {
	var ctx = dt.settings()[0];
	var init = ctx.oInit.select;
	var defaults = DataTable.defaults.select;
	var opts = init === undefined ?
		defaults :
		init;

	// Set defaults
	var items = 'row';
	var style = 'api';
	var blurable = false;
	var info = true;
	var selector = 'td, th';
	var className = 'selected';
	var setStyle = false;

	ctx._select = {};

	// Initialisation customisations
	if ( opts === true ) {
		style = 'os';
		setStyle = true;
	}
	else if ( typeof opts === 'string' ) {
		style = opts;
		setStyle = true;
	}
	else if ( $.isPlainObject( opts ) ) {
		if ( opts.blurable !== undefined ) {
			blurable = opts.blurable;
		}

		if ( opts.info !== undefined ) {
			info = opts.info;
		}

		if ( opts.items !== undefined ) {
			items = opts.items;
		}

		if ( opts.style !== undefined ) {
			style = opts.style;
			setStyle = true;
		}

		if ( opts.selector !== undefined ) {
			selector = opts.selector;
		}

		if ( opts.className !== undefined ) {
			className = opts.className;
		}
	}

	dt.select.selector( selector );
	dt.select.items( items );
	dt.select.style( style );
	dt.select.blurable( blurable );
	dt.select.info( info );
	ctx._select.className = className;


	// Sort table based on selected rows. Requires Select Datatables extension
	$.fn.dataTable.ext.order['select-checkbox'] = function ( settings, col ) {
		return this.api().column( col, {order: 'index'} ).nodes().map( function ( td ) {
			if ( settings._select.items === 'row' ) {
				return $( td ).parent().hasClass( settings._select.className );
			} else if ( settings._select.items === 'cell' ) {
				return $( td ).hasClass( settings._select.className );
			}
			return false;
		});
	};

	// If the init options haven't enabled select, but there is a selectable
	// class name, then enable
	if ( ! setStyle && $( dt.table().node() ).hasClass( 'selectable' ) ) {
		dt.select.style( 'os' );
	}
};

/*

Select is a collection of API methods, event handlers, event emitters and
buttons (for the `Buttons` extension) for DataTables. It provides the following
features, with an overview of how they are implemented:

## Selection of rows, columns and cells. Whether an item is selected or not is
   stored in:

* rows: a `_select_selected` property which contains a boolean value of the
  DataTables' `aoData` object for each row
* columns: a `_select_selected` property which contains a boolean value of the
  DataTables' `aoColumns` object for each column
* cells: a `_selected_cells` property which contains an array of boolean values
  of the `aoData` object for each row. The array is the same length as the
  columns array, with each element of it representing a cell.

This method of using boolean flags allows Select to operate when nodes have not
been created for rows / cells (DataTables' defer rendering feature).

## API methods

A range of API methods are available for triggering selection and de-selection
of rows. Methods are also available to configure the selection events that can
be triggered by an end user (such as which items are to be selected). To a large
extent, these of API methods *is* Select. It is basically a collection of helper
functions that can be used to select items in a DataTable.

Configuration of select is held in the object `_select` which is attached to the
DataTables settings object on initialisation. Select being available on a table
is not optional when Select is loaded, but its default is for selection only to
be available via the API - so the end user wouldn't be able to select rows
without additional configuration.

The `_select` object contains the following properties:

```
{
	items:string     - Can be `rows`, `columns` or `cells`. Defines what item 
	                   will be selected if the user is allowed to activate row
	                   selection using the mouse.
	style:string     - Can be `none`, `single`, `multi` or `os`. Defines the
	                   interaction style when selecting items
	blurable:boolean - If row selection can be cleared by clicking outside of
	                   the table
	info:boolean     - If the selection summary should be shown in the table
	                   information elements
}
```

In addition to the API methods, Select also extends the DataTables selector
options for rows, columns and cells adding a `selected` option to the selector
options object, allowing the developer to select only selected items or
unselected items.

## Mouse selection of items

Clicking on items can be used to select items. This is done by a simple event
handler that will select the items using the API methods.

 */


/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Local functions
 */

/**
 * Add one or more cells to the selection when shift clicking in OS selection
 * style cell selection.
 *
 * Cell range is more complicated than row and column as we want to select
 * in the visible grid rather than by index in sequence. For example, if you
 * click first in cell 1-1 and then shift click in 2-2 - cells 1-2 and 2-1
 * should also be selected (and not 1-3, 1-4. etc)
 * 
 * @param  {DataTable.Api} dt   DataTable
 * @param  {object}        idx  Cell index to select to
 * @param  {object}        last Cell index to select from
 * @private
 */
function cellRange( dt, idx, last )
{
	var indexes;
	var columnIndexes;
	var rowIndexes;
	var selectColumns = function ( start, end ) {
		if ( start > end ) {
			var tmp = end;
			end = start;
			start = tmp;
		}
		
		var record = false;
		return dt.columns( ':visible' ).indexes().filter( function (i) {
			if ( i === start ) {
				record = true;
			}
			
			if ( i === end ) { // not else if, as start might === end
				record = false;
				return true;
			}

			return record;
		} );
	};

	var selectRows = function ( start, end ) {
		var indexes = dt.rows( { search: 'applied' } ).indexes();

		// Which comes first - might need to swap
		if ( indexes.indexOf( start ) > indexes.indexOf( end ) ) {
			var tmp = end;
			end = start;
			start = tmp;
		}

		var record = false;
		return indexes.filter( function (i) {
			if ( i === start ) {
				record = true;
			}
			
			if ( i === end ) {
				record = false;
				return true;
			}

			return record;
		} );
	};

	if ( ! dt.cells( { selected: true } ).any() && ! last ) {
		// select from the top left cell to this one
		columnIndexes = selectColumns( 0, idx.column );
		rowIndexes = selectRows( 0 , idx.row );
	}
	else {
		// Get column indexes between old and new
		columnIndexes = selectColumns( last.column, idx.column );
		rowIndexes = selectRows( last.row , idx.row );
	}

	indexes = dt.cells( rowIndexes, columnIndexes ).flatten();

	if ( ! dt.cells( idx, { selected: true } ).any() ) {
		// Select range
		dt.cells( indexes ).select();
	}
	else {
		// Deselect range
		dt.cells( indexes ).deselect();
	}
}

/**
 * Disable mouse selection by removing the selectors
 *
 * @param {DataTable.Api} dt DataTable to remove events from
 * @private
 */
function disableMouseSelection( dt )
{
	var ctx = dt.settings()[0];
	var selector = ctx._select.selector;

	$( dt.table().container() )
		.off( 'mousedown.dtSelect', selector )
		.off( 'mouseup.dtSelect', selector )
		.off( 'click.dtSelect', selector );

	$('body').off( 'click.dtSelect' + dt.table().node().id );
}

/**
 * Attach mouse listeners to the table to allow mouse selection of items
 *
 * @param {DataTable.Api} dt DataTable to remove events from
 * @private
 */
function enableMouseSelection ( dt )
{
	var container = $( dt.table().container() );
	var ctx = dt.settings()[0];
	var selector = ctx._select.selector;

	container
		.on( 'mousedown.dtSelect', selector, function(e) {
			// Disallow text selection for shift clicking on the table so multi
			// element selection doesn't look terrible!
			if ( e.shiftKey || e.metaKey || e.ctrlKey ) {
				container
					.css( '-moz-user-select', 'none' )
					.one('selectstart.dtSelect', selector, function () {
						return false;
					} );
			}
		} )
		.on( 'mouseup.dtSelect', selector, function() {
			// Allow text selection to occur again, Mozilla style (tested in FF
			// 35.0.1 - still required)
			container.css( '-moz-user-select', '' );
		} )
		.on( 'click.dtSelect', selector, function ( e ) {
			var items = dt.select.items();
			var idx;

			// If text was selected (click and drag), then we shouldn't change
			// the row's selected state
			if ( window.getSelection ) {
				var selection = window.getSelection();

				// If the element that contains the selection is not in the table, we can ignore it
				// This can happen if the developer selects text from the click event
				if ( ! selection.anchorNode || $(selection.anchorNode).closest('table')[0] === dt.table().node() ) {
					if ( $.trim(selection.toString()) !== '' ) {
						return;
					}
				}
			}

			var ctx = dt.settings()[0];

			// Ignore clicks inside a sub-table
			if ( $(e.target).closest('div.dataTables_wrapper')[0] != dt.table().container() ) {
				return;
			}

			var cell = dt.cell( $(e.target).closest('td, th') );

			// Check the cell actually belongs to the host DataTable (so child
			// rows, etc, are ignored)
			if ( ! cell.any() ) {
				return;
			}

			var event = $.Event('user-select.dt');
			eventTrigger( dt, event, [ items, cell, e ] );

			if ( event.isDefaultPrevented() ) {
				return;
			}

			var cellIndex = cell.index();
			if ( items === 'row' ) {
				idx = cellIndex.row;
				typeSelect( e, dt, ctx, 'row', idx );
			}
			else if ( items === 'column' ) {
				idx = cell.index().column;
				typeSelect( e, dt, ctx, 'column', idx );
			}
			else if ( items === 'cell' ) {
				idx = cell.index();
				typeSelect( e, dt, ctx, 'cell', idx );
			}

			ctx._select_lastCell = cellIndex;
		} );

	// Blurable
	$('body').on( 'click.dtSelect' + dt.table().node().id, function ( e ) {
		if ( ctx._select.blurable ) {
			// If the click was inside the DataTables container, don't blur
			if ( $(e.target).parents().filter( dt.table().container() ).length ) {
				return;
			}

			// Ignore elements which have been removed from the DOM (i.e. paging
			// buttons)
			if ( $(e.target).parents('html').length === 0 ) {
			 	return;
			}

			// Don't blur in Editor form
			if ( $(e.target).parents('div.DTE').length ) {
				return;
			}

			clear( ctx, true );
		}
	} );
}

/**
 * Trigger an event on a DataTable
 *
 * @param {DataTable.Api} api      DataTable to trigger events on
 * @param  {boolean}      selected true if selected, false if deselected
 * @param  {string}       type     Item type acting on
 * @param  {boolean}      any      Require that there are values before
 *     triggering
 * @private
 */
function eventTrigger ( api, type, args, any )
{
	if ( any && ! api.flatten().length ) {
		return;
	}

	if ( typeof type === 'string' ) {
		type = type +'.dt';
	}

	args.unshift( api );

	$(api.table().node()).trigger( type, args );
}

/**
 * Update the information element of the DataTable showing information about the
 * items selected. This is done by adding tags to the existing text
 * 
 * @param {DataTable.Api} api DataTable to update
 * @private
 */
function info ( api )
{
	var ctx = api.settings()[0];

	if ( ! ctx._select.info || ! ctx.aanFeatures.i ) {
		return;
	}

	if ( api.select.style() === 'api' ) {
		return;
	}

	var rows    = api.rows( { selected: true } ).flatten().length;
	var columns = api.columns( { selected: true } ).flatten().length;
	var cells   = api.cells( { selected: true } ).flatten().length;

	var add = function ( el, name, num ) {
		el.append( $('<span class="select-item"/>').append( api.i18n(
			'select.'+name+'s',
			{ _: '%d '+name+'s selected', 0: '', 1: '1 '+name+' selected' },
			num
		) ) );
	};

	// Internal knowledge of DataTables to loop over all information elements
	$.each( ctx.aanFeatures.i, function ( i, el ) {
		el = $(el);

		var output  = $('<span class="select-info"/>');
		add( output, 'row', rows );
		add( output, 'column', columns );
		add( output, 'cell', cells  );

		var exisiting = el.children('span.select-info');
		if ( exisiting.length ) {
			exisiting.remove();
		}

		if ( output.text() !== '' ) {
			el.append( output );
		}
	} );
}

/**
 * Initialisation of a new table. Attach event handlers and callbacks to allow
 * Select to operate correctly.
 *
 * This will occur _after_ the initial DataTables initialisation, although
 * before Ajax data is rendered, if there is ajax data
 *
 * @param  {DataTable.settings} ctx Settings object to operate on
 * @private
 */
function init ( ctx ) {
	var api = new DataTable.Api( ctx );

	// Row callback so that classes can be added to rows and cells if the item
	// was selected before the element was created. This will happen with the
	// `deferRender` option enabled.
	// 
	// This method of attaching to `aoRowCreatedCallback` is a hack until
	// DataTables has proper events for row manipulation If you are reviewing
	// this code to create your own plug-ins, please do not do this!
	ctx.aoRowCreatedCallback.push( {
		fn: function ( row, data, index ) {
			var i, ien;
			var d = ctx.aoData[ index ];

			// Row
			if ( d._select_selected ) {
				$( row ).addClass( ctx._select.className );
			}

			// Cells and columns - if separated out, we would need to do two
			// loops, so it makes sense to combine them into a single one
			for ( i=0, ien=ctx.aoColumns.length ; i<ien ; i++ ) {
				if ( ctx.aoColumns[i]._select_selected || (d._selected_cells && d._selected_cells[i]) ) {
					$(d.anCells[i]).addClass( ctx._select.className );
				}
			}
		},
		sName: 'select-deferRender'
	} );

	// On Ajax reload we want to reselect all rows which are currently selected,
	// if there is an rowId (i.e. a unique value to identify each row with)
	api.on( 'preXhr.dt.dtSelect', function () {
		// note that column selection doesn't need to be cached and then
		// reselected, as they are already selected
		var rows = api.rows( { selected: true } ).ids( true ).filter( function ( d ) {
			return d !== undefined;
		} );

		var cells = api.cells( { selected: true } ).eq(0).map( function ( cellIdx ) {
			var id = api.row( cellIdx.row ).id( true );
			return id ?
				{ row: id, column: cellIdx.column } :
				undefined;
		} ).filter( function ( d ) {
			return d !== undefined;
		} );

		// On the next draw, reselect the currently selected items
		api.one( 'draw.dt.dtSelect', function () {
			api.rows( rows ).select();

			// `cells` is not a cell index selector, so it needs a loop
			if ( cells.any() ) {
				cells.each( function ( id ) {
					api.cells( id.row, id.column ).select();
				} );
			}
		} );
	} );

	// Update the table information element with selected item summary
	api.on( 'draw.dtSelect.dt select.dtSelect.dt deselect.dtSelect.dt info.dt', function () {
		info( api );
	} );

	// Clean up and release
	api.on( 'destroy.dtSelect', function () {
		disableMouseSelection( api );
		api.off( '.dtSelect' );
	} );
}

/**
 * Add one or more items (rows or columns) to the selection when shift clicking
 * in OS selection style
 *
 * @param  {DataTable.Api} dt   DataTable
 * @param  {string}        type Row or column range selector
 * @param  {object}        idx  Item index to select to
 * @param  {object}        last Item index to select from
 * @private
 */
function rowColumnRange( dt, type, idx, last )
{
	// Add a range of rows from the last selected row to this one
	var indexes = dt[type+'s']( { search: 'applied' } ).indexes();
	var idx1 = $.inArray( last, indexes );
	var idx2 = $.inArray( idx, indexes );

	if ( ! dt[type+'s']( { selected: true } ).any() && idx1 === -1 ) {
		// select from top to here - slightly odd, but both Windows and Mac OS
		// do this
		indexes.splice( $.inArray( idx, indexes )+1, indexes.length );
	}
	else {
		// reverse so we can shift click 'up' as well as down
		if ( idx1 > idx2 ) {
			var tmp = idx2;
			idx2 = idx1;
			idx1 = tmp;
		}

		indexes.splice( idx2+1, indexes.length );
		indexes.splice( 0, idx1 );
	}

	if ( ! dt[type]( idx, { selected: true } ).any() ) {
		// Select range
		dt[type+'s']( indexes ).select();
	}
	else {
		// Deselect range - need to keep the clicked on row selected
		indexes.splice( $.inArray( idx, indexes ), 1 );
		dt[type+'s']( indexes ).deselect();
	}
}

/**
 * Clear all selected items
 *
 * @param  {DataTable.settings} ctx Settings object of the host DataTable
 * @param  {boolean} [force=false] Force the de-selection to happen, regardless
 *     of selection style
 * @private
 */
function clear( ctx, force )
{
	if ( force || ctx._select.style === 'single' ) {
		var api = new DataTable.Api( ctx );
		
		api.rows( { selected: true } ).deselect();
		api.columns( { selected: true } ).deselect();
		api.cells( { selected: true } ).deselect();
	}
}

/**
 * Select items based on the current configuration for style and items.
 *
 * @param  {object}             e    Mouse event object
 * @param  {DataTables.Api}     dt   DataTable
 * @param  {DataTable.settings} ctx  Settings object of the host DataTable
 * @param  {string}             type Items to select
 * @param  {int|object}         idx  Index of the item to select
 * @private
 */
function typeSelect ( e, dt, ctx, type, idx )
{
	var style = dt.select.style();
	var isSelected = dt[type]( idx, { selected: true } ).any();

	if ( style === 'os' ) {
		if ( e.ctrlKey || e.metaKey ) {
			// Add or remove from the selection
			dt[type]( idx ).select( ! isSelected );
		}
		else if ( e.shiftKey ) {
			if ( type === 'cell' ) {
				cellRange( dt, idx, ctx._select_lastCell || null );
			}
			else {
				rowColumnRange( dt, type, idx, ctx._select_lastCell ?
					ctx._select_lastCell[type] :
					null
				);
			}
		}
		else {
			// No cmd or shift click - deselect if selected, or select
			// this row only
			var selected = dt[type+'s']( { selected: true } );

			if ( isSelected && selected.flatten().length === 1 ) {
				dt[type]( idx ).deselect();
			}
			else {
				selected.deselect();
				dt[type]( idx ).select();
			}
		}
	} else if ( style == 'multi+shift' ) {
		if ( e.shiftKey ) {
			if ( type === 'cell' ) {
				cellRange( dt, idx, ctx._select_lastCell || null );
			}
			else {
				rowColumnRange( dt, type, idx, ctx._select_lastCell ?
					ctx._select_lastCell[type] :
					null
				);
			}
		}
		else {
			dt[ type ]( idx ).select( ! isSelected );
		}
	}
	else {
		dt[ type ]( idx ).select( ! isSelected );
	}
}



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables selectors
 */

// row and column are basically identical just assigned to different properties
// and checking a different array, so we can dynamically create the functions to
// reduce the code size
$.each( [
	{ type: 'row', prop: 'aoData' },
	{ type: 'column', prop: 'aoColumns' }
], function ( i, o ) {
	DataTable.ext.selector[ o.type ].push( function ( settings, opts, indexes ) {
		var selected = opts.selected;
		var data;
		var out = [];

		if ( selected === undefined ) {
			return indexes;
		}

		for ( var i=0, ien=indexes.length ; i<ien ; i++ ) {
			data = settings[ o.prop ][ indexes[i] ];

			if ( (selected === true && data._select_selected === true) ||
			     (selected === false && ! data._select_selected )
			) {
				out.push( indexes[i] );
			}
		}

		return out;
	} );
} );

DataTable.ext.selector.cell.push( function ( settings, opts, cells ) {
	var selected = opts.selected;
	var rowData;
	var out = [];

	if ( selected === undefined ) {
		return cells;
	}

	for ( var i=0, ien=cells.length ; i<ien ; i++ ) {
		rowData = settings.aoData[ cells[i].row ];

		if ( (selected === true && rowData._selected_cells && rowData._selected_cells[ cells[i].column ] === true) ||
		     (selected === false && ( ! rowData._selected_cells || ! rowData._selected_cells[ cells[i].column ] ) )
		) {
			out.push( cells[i] );
		}
	}

	return out;
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * DataTables API
 *
 * For complete documentation, please refer to the docs/api directory or the
 * DataTables site
 */

// Local variables to improve compression
var apiRegister = DataTable.Api.register;
var apiRegisterPlural = DataTable.Api.registerPlural;

apiRegister( 'select()', function () {
	return this.iterator( 'table', function ( ctx ) {
		DataTable.select.init( new DataTable.Api( ctx ) );
	} );
} );

apiRegister( 'select.blurable()', function ( flag ) {
	if ( flag === undefined ) {
		return this.context[0]._select.blurable;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.blurable = flag;
	} );
} );

apiRegister( 'select.info()', function ( flag ) {
	if ( info === undefined ) {
		return this.context[0]._select.info;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.info = flag;
	} );
} );

apiRegister( 'select.items()', function ( items ) {
	if ( items === undefined ) {
		return this.context[0]._select.items;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.items = items;

		eventTrigger( new DataTable.Api( ctx ), 'selectItems', [ items ] );
	} );
} );

// Takes effect from the _next_ selection. None disables future selection, but
// does not clear the current selection. Use the `deselect` methods for that
apiRegister( 'select.style()', function ( style ) {
	if ( style === undefined ) {
		return this.context[0]._select.style;
	}

	return this.iterator( 'table', function ( ctx ) {
		ctx._select.style = style;

		if ( ! ctx._select_init ) {
			init( ctx );
		}

		// Add / remove mouse event handlers. They aren't required when only
		// API selection is available
		var dt = new DataTable.Api( ctx );
		disableMouseSelection( dt );
		
		if ( style !== 'api' ) {
			enableMouseSelection( dt );
		}

		eventTrigger( new DataTable.Api( ctx ), 'selectStyle', [ style ] );
	} );
} );

apiRegister( 'select.selector()', function ( selector ) {
	if ( selector === undefined ) {
		return this.context[0]._select.selector;
	}

	return this.iterator( 'table', function ( ctx ) {
		disableMouseSelection( new DataTable.Api( ctx ) );

		ctx._select.selector = selector;

		if ( ctx._select.style !== 'api' ) {
			enableMouseSelection( new DataTable.Api( ctx ) );
		}
	} );
} );



apiRegisterPlural( 'rows().select()', 'row().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'row', function ( ctx, idx ) {
		clear( ctx );

		ctx.aoData[ idx ]._select_selected = true;
		$( ctx.aoData[ idx ].nTr ).addClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'row', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'columns().select()', 'column().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'column', function ( ctx, idx ) {
		clear( ctx );

		ctx.aoColumns[ idx ]._select_selected = true;

		var column = new DataTable.Api( ctx ).column( idx );

		$( column.header() ).addClass( ctx._select.className );
		$( column.footer() ).addClass( ctx._select.className );

		column.nodes().to$().addClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'column', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'cells().select()', 'cell().select()', function ( select ) {
	var api = this;

	if ( select === false ) {
		return this.deselect();
	}

	this.iterator( 'cell', function ( ctx, rowIdx, colIdx ) {
		clear( ctx );

		var data = ctx.aoData[ rowIdx ];

		if ( data._selected_cells === undefined ) {
			data._selected_cells = [];
		}

		data._selected_cells[ colIdx ] = true;

		if ( data.anCells ) {
			$( data.anCells[ colIdx ] ).addClass( ctx._select.className );
		}
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'select', [ 'cell', api[i] ], true );
	} );

	return this;
} );


apiRegisterPlural( 'rows().deselect()', 'row().deselect()', function () {
	var api = this;

	this.iterator( 'row', function ( ctx, idx ) {
		ctx.aoData[ idx ]._select_selected = false;
		$( ctx.aoData[ idx ].nTr ).removeClass( ctx._select.className );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'row', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'columns().deselect()', 'column().deselect()', function () {
	var api = this;

	this.iterator( 'column', function ( ctx, idx ) {
		ctx.aoColumns[ idx ]._select_selected = false;

		var api = new DataTable.Api( ctx );
		var column = api.column( idx );

		$( column.header() ).removeClass( ctx._select.className );
		$( column.footer() ).removeClass( ctx._select.className );

		// Need to loop over each cell, rather than just using
		// `column().nodes()` as cells which are individually selected should
		// not have the `selected` class removed from them
		api.cells( null, idx ).indexes().each( function (cellIdx) {
			var data = ctx.aoData[ cellIdx.row ];
			var cellSelected = data._selected_cells;

			if ( data.anCells && (! cellSelected || ! cellSelected[ cellIdx.column ]) ) {
				$( data.anCells[ cellIdx.column  ] ).removeClass( ctx._select.className );
			}
		} );
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'column', api[i] ], true );
	} );

	return this;
} );

apiRegisterPlural( 'cells().deselect()', 'cell().deselect()', function () {
	var api = this;

	this.iterator( 'cell', function ( ctx, rowIdx, colIdx ) {
		var data = ctx.aoData[ rowIdx ];

		data._selected_cells[ colIdx ] = false;

		// Remove class only if the cells exist, and the cell is not column
		// selected, in which case the class should remain (since it is selected
		// in the column)
		if ( data.anCells && ! ctx.aoColumns[ colIdx ]._select_selected ) {
			$( data.anCells[ colIdx ] ).removeClass( ctx._select.className );
		}
	} );

	this.iterator( 'table', function ( ctx, i ) {
		eventTrigger( api, 'deselect', [ 'cell', api[i] ], true );
	} );

	return this;
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Buttons
 */
function i18n( label, def ) {
	return function (dt) {
		return dt.i18n( 'buttons.'+label, def );
	};
}

// Common events with suitable namespaces
function namespacedEvents ( config ) {
	var unique = config._eventNamespace;

	return 'draw.dt.DT'+unique+' select.dt.DT'+unique+' deselect.dt.DT'+unique;
}

var _buttonNamespace = 0;

$.extend( DataTable.ext.buttons, {
	selected: {
		text: i18n( 'selected', 'Selected' ),
		className: 'buttons-selected',
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			// .DT namespace listeners are removed by DataTables automatically
			// on table destroy
			dt.on( namespacedEvents(config), function () {
				var enable = that.rows( { selected: true } ).any() ||
				             that.columns( { selected: true } ).any() ||
				             that.cells( { selected: true } ).any();

				that.enable( enable );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	},
	selectedSingle: {
		text: i18n( 'selectedSingle', 'Selected single' ),
		className: 'buttons-selected-single',
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			dt.on( namespacedEvents(config), function () {
				var count = dt.rows( { selected: true } ).flatten().length +
				            dt.columns( { selected: true } ).flatten().length +
				            dt.cells( { selected: true } ).flatten().length;

				that.enable( count === 1 );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	},
	selectAll: {
		text: i18n( 'selectAll', 'Select all' ),
		className: 'buttons-select-all',
		action: function () {
			var items = this.select.items();
			this[ items+'s' ]().select();
		}
	},
	selectNone: {
		text: i18n( 'selectNone', 'Deselect all' ),
		className: 'buttons-select-none',
		action: function () {
			clear( this.settings()[0], true );
		},
		init: function ( dt, node, config ) {
			var that = this;
			config._eventNamespace = '.select'+(_buttonNamespace++);

			dt.on( namespacedEvents(config), function () {
				var count = dt.rows( { selected: true } ).flatten().length +
				            dt.columns( { selected: true } ).flatten().length +
				            dt.cells( { selected: true } ).flatten().length;

				that.enable( count > 0 );
			} );

			this.disable();
		},
		destroy: function ( dt, node, config ) {
			dt.off( config._eventNamespace );
		}
	}
} );

$.each( [ 'Row', 'Column', 'Cell' ], function ( i, item ) {
	var lc = item.toLowerCase();

	DataTable.ext.buttons[ 'select'+item+'s' ] = {
		text: i18n( 'select'+item+'s', 'Select '+lc+'s' ),
		className: 'buttons-select-'+lc+'s',
		action: function () {
			this.select.items( lc );
		},
		init: function ( dt ) {
			var that = this;

			dt.on( 'selectItems.dt.DT', function ( e, ctx, items ) {
				that.active( items === lc );
			} );
		}
	};
} );



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Initialisation
 */

// DataTables creation - check if select has been defined in the options. Note
// this required that the table be in the document! If it isn't then something
// needs to trigger this method unfortunately. The next major release of
// DataTables will rework the events and address this.
$(document).on( 'preInit.dt.dtSelect', function (e, ctx) {
	if ( e.namespace !== 'dt' ) {
		return;
	}

	DataTable.select.init( new DataTable.Api( ctx ) );
} );


return DataTable.select;
}));


