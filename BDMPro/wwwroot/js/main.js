//show back to top button at bottom right corner after the page scrolled
const topnav = document.querySelector("#top-navigation");
void 0 !== topnav && null != topnav && document.addEventListener("scroll", () => {
    window.scrollY > 99 ? topnav.classList.add("scrolled") : topnav.classList.remove("scrolled"), topnav.classList.contains("scrolled-shadow") && (window.scrollY > 100 ? topnav.classList.add("shadow") : topnav.classList.remove("shadow"))
});
const scrollTop = document.querySelector(".scroll-top");
if (void 0 !== scrollTop && null != scrollTop) {
    let d = function () {
        window.scrollY > 99 ? scrollTop.classList.add("active") : scrollTop.classList.remove("active")
    },
        g = function () {
            window.scrollTo({
                top: 0,
                behavior: "smooth"
            })
        };
    window.addEventListener("load", d), document.addEventListener("scroll", d), scrollTop.addEventListener("click", g)
}

//hide the dummy header and footer of a table after finish loading table data
function hideDummySpinnerHeaderFooter(tablewrapperid) {
    let spinner = document.querySelector("#" + tablewrapperid + " .spinner");
    let dummyFooter = document.querySelector("#" + tablewrapperid + " .dummyfooter");
    spinner.classList.remove("d-flex");
    spinner.classList.add("d-none");
    dummyFooter.classList.add("d-none");
}

//set background image of div with data-img tag
var bgimg = document.querySelector("[data-img]");
if (void 0 !== bgimg && null != bgimg) {
    for (var element, dataimgs = document.querySelectorAll("[data-img]"), i = 0; element = dataimgs[i]; i++) {
        var h = element.getAttribute("data-img"),
            a = element.getAttribute("data-img-position"),
            b = element.getAttribute("data-img-attachment");
        element.style.background = "url(" + h + ")", void 0 !== a && null != a ? element.style.backgroundPosition = a : element.style.backgroundPosition = "center center", void 0 !== b && null != b ? element.style.backgroundAttachment = b : element.style.backgroundAttachment = "scroll", element.style.backgroundSize = "cover", element.style.backgroundRepeat = "no-repeat"
    }
}

// self executing function
(function () {
    initDropdownlist();
    formatDateText();
    formatDateTimeText();
    initDateInputs();
    initDateTimeInputs();
})();

function handleDropdownSelection(event) {
    // Get the selected dropdown item
    const selected = event.target;
    // Get the parent dropdown
    const dropdown = selected.closest('.form-dropdown');
    // Get the input element
    const input = dropdown.querySelector('.form-dropdown .dropdown-input');
    // Set the input value to the selected value
    input.value = selected.dataset.value;
    // Remove active class from all items and add to selected
    dropdown.querySelectorAll('.dropdown-item').forEach(item => item.classList.remove('active'));
    selected.classList.add('active');
    // Set the dropdown toggle text to the selected text
    const dropdownToggle = dropdown.querySelector('.form-dropdown .dropdown-toggle');
    dropdownToggle.innerText = selected.innerText;
}

//initialize drop down list
function initDropdownlist() {

    // Get all dropdown items on the page
    const dropdownItems = document.querySelectorAll('.form-dropdown .dropdown-item');
    if (void 0 !== dropdownItems && null != dropdownItems) {
        // Add a click event listener to each dropdown item
        dropdownItems.forEach(item => {
            item.addEventListener('click', handleDropdownSelection);
        });
    }

    var searchInputs = document.querySelectorAll('.form-dropdown .dropdown-menu input');
    for (var i = 0; i < searchInputs.length; i++) {
        var searchInput = searchInputs[i];
        if (searchInput) {
            searchInput.addEventListener('click', function (event) {
                event.stopPropagation();
            });
            searchInput.addEventListener('input', function () {
                var searchQuery = this.value.toLowerCase();
                //console.log(searchQuery + ' searchQuery');
                // Get a reference to the parent ul element
                var ul = this.closest('ul');
                // Get all the li elements inside the ul except the one containing the input
                var siblings = Array.prototype.filter.call(ul.children, function (li) {
                    return li !== searchInput.parentNode && li !== ul.firstElementChild;
                });
                // Do something with the siblings
                for (var j = 0; j < siblings.length; j++) {
                    // Do something with each sibling
                    var optionText = siblings[j].textContent.toLowerCase();
                    if (optionText.indexOf(searchQuery) === -1 && siblings[j].dataset.value !== 'null') {
                        siblings[j].style.display = 'none';
                    } else {
                        siblings[j].style.display = 'block';
                    }
                }
            });
        }
    }
}

function changeDropDownListItems(childDropDownElementId, selectList, placeholder) {
    // Get a reference to the dropdown element
    var dropdown = document.querySelector(`#${childDropDownElementId}`);
    var btn = dropdown.querySelector("button");
    btn.innerText = placeholder;
    var dropdowninput = dropdown.querySelector("input.dropdown-input");
    dropdowninput.value = "";
    // Get a reference to the ul element inside the dropdown
    var ul = dropdown.querySelector('ul');
    // Get all the li elements with class "dropdown-item"
    var liItems = ul.querySelectorAll('.dropdown-list');
    // Loop through the li elements and remove them from the DOM
    for (var i = 0; i < liItems.length; i++) {
        liItems[i].remove();
    }
    $.each(selectList, function (index, element) {
        ul.innerHTML += `<li class='dropdown-list'><a class='dropdown-item' data-value='${element.Value}'>${element.Text}</a></li>`;
    });
    initDropdownlist();
}

//this will reset drop down list to placeholder text
function resetDropDown(dropdownId, placeholder) {
    // Get all the dropdown items
    var dropdownItems = document.querySelectorAll(`#${dropdownId}-ddl .dropdown-item`);
    if (dropdownItems != undefined) {
        var searchInputs = document.querySelector(`#${dropdownId}-ddl .dropdown-menu input`);
        if (searchInputs != undefined) {
            searchInputs.values = "";
        }
        var activeitem = document.querySelector(`#${dropdownId}-ddl .dropdown-item.active`);
        if (activeitem != undefined) {
            activeitem.classList.remove('active');
        }
        // Set the dropdown toggle text to the selected text
        const dropdownToggle = document.querySelector(`#${dropdownId}-ddl .dropdown-toggle`);
        if (dropdownToggle != undefined) {
            dropdownToggle.innerText = placeholder;
        }
        //set dropdown input value to ""
        const dropdownInputValue = document.querySelector(`input.dropdown-input[name='${dropdownId}']`);
        if (dropdownInputValue != undefined) {
            dropdownInputValue.value = "";
        }
    }
}

function resetTable(tableIdPrefix) {
    $(".spinner-container").removeClass("d-none");
    var searchinput = document.querySelector(`#${tableIdPrefix}-searchfilter input[name="search"]`);
    if (searchinput != null) {
        searchinput.value = "";
    }
    //all Dropdown Within Search Filter Container
    const filterDropdowns = document.querySelectorAll(`#${tableIdPrefix}-searchfilter .dropdown`);
    if (filterDropdowns != null) {
        filterDropdowns.forEach(dropdown => {
            //reset the search input in the drop down
            const searchInputs = dropdown.querySelector(`.dropdown-menu input`);
            if (searchInputs != null) {
                searchInputs.value = "";
            }
            //reset the <li> options
            var ul = dropdown.querySelector(`ul`);
            var siblings = Array.prototype.filter.call(ul.children, function (li) {
                return li;
            });
            for (var j = 0; j < siblings.length; j++) {
                siblings[j].style.display = 'block';
            }
            //remove the active class of the selected <li> option
            const activeitem = dropdown.querySelector(`.dropdown-item.active`);
            if (activeitem != null) {
                activeitem.classList.remove('active');
            }
            // Set the dropdown toggle text to the selected text
            const dropdownToggle = dropdown.querySelector(`.dropdown-toggle`);
            if (dropdownToggle != null) {
                const placeholdertext = dropdown.querySelector(`button`).getAttribute("placeholder");
                if (placeholdertext != null) {
                    dropdownToggle.textContent = placeholdertext;
                } else {
                    dropdownToggle.textContent = "Please Select";
                }
                const selectedDropdownItem = document.querySelector('.dropdown-item');
                if (selectedDropdownItem != null && selectedDropdownItem.innerText.trim() === 'Show 10 Records') {
                    selectedDropdownItem.classList.add('active');
                }
            }
            //set dropdown input value to ""
            const dropdownInputValue = dropdown.querySelector(`input.dropdown-input`);
            if (dropdownInputValue != null) {
                dropdownInputValue.value = "";
            }
        });
    }
}

function loadData(loadTableUrl, searchFilterParams, tableIdPrefix) {
    fetch(loadTableUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(searchFilterParams)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then(data => {
            $(`#${tableIdPrefix}-mainwrapper`).html(data);
            formatDateTimeText();
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        })
        .finally(() => {
            $(".spinner-container").addClass("d-none");
        });
}

//this function will adjust the table that export to pdf to fit the width of pdf
function adjustPdfColWidth(tableIdPrefix) {
    var colCount = new Array();
    $('#' + tableIdPrefix + '-table').find('tbody tr:first-child td').each(function () {
        let col = $(this).html();
        if (col.includes("actioncol") == false) {
            if ($(this).attr('colspan')) {
                for (var i = 1; i <= $(this).attr('colspan'); $i++) {
                    colCount.push('*');
                }
            }
            else { colCount.push('*'); }
        }
    });
    return colCount;
}

//this function will exclude the action column when export to pdf/excel
function getTotalColumns(tableIdPrefix) {
    var colsToBeExported = new Array();
    var count = 0;
    $('#' + tableIdPrefix + '-table').find('tbody tr:first-child td').each(function () {
        colsToBeExported.push(count);
        count++;
    });
    return colsToBeExported;
}

function initDateInputs() {
    // Get all the input elements of type 'date'
    const dateInputs = document.querySelectorAll('input[type="date"]');
    if (dateInputs.length > 0) {
        // Attach event listeners to each date input
        dateInputs.forEach(function (input) {
            input.addEventListener('change', function () {
                setDateValue(input);
            });
        });
    }
}

function initDateTimeInputs() {
    // Get all the input elements of type 'date'
    const datetimeInputs = document.querySelectorAll('input[type="datetime-local"]');
    if (datetimeInputs.length > 0) {
        // Attach event listeners to each date input
        datetimeInputs.forEach(function (input) {
            input.addEventListener('change', function () {
                setDateTimeValue(input);
            });
        });
    }
}

function setDateValue(input) {
    const isoUtcString = input.value;
    if (isoUtcString != "" && isoUtcString != null) {
        const date = new Date(isoUtcString);
        const localDate = date.toISOString().substring(0, 10);
        input.value = localDate;
        input.setAttribute("value", localDate);
    }
}

function setDateTimeValue(input) {
    const isoUtcString = input.value;
    if (isoUtcString != "" && isoUtcString != null) {
        const date = new Date(isoUtcString);
        const localDateTime = date.toISOString().substring(0, 16);
        input.value = localDateTime;
        input.setAttribute("value", localDateTime);
    }
}

function convertToLocalDateIsoString(isoUtcString) {
    const dateTimeUtc = new Date(isoUtcString);
    var options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    var dateString = dateTimeUtc.toLocaleDateString(undefined, options);
    var datePart = dateString.substring(0, 10);
    return datePart;
}

function convertToLocalDatetimeIsoString(isoUtcString) {
    const dateTimeUtc = new Date(isoUtcString);
    const dateTimeLocal = new Date(dateTimeUtc.getTime() - dateTimeUtc.getTimezoneOffset() * 60 * 1000);
    const formattedDateTime = dateTimeLocal.getFullYear() + '-' +
        (dateTimeLocal.getMonth() + 1).toString().padStart(2, '0') + '-' +
        dateTimeLocal.getDate().toString().padStart(2, '0') + 'T' +
        dateTimeLocal.getHours().toString().padStart(2, '0') + ':' +
        dateTimeLocal.getMinutes().toString().padStart(2, '0');
    return formattedDateTime; //example: 2023-02-11T14:44 (this format used in <input type="datetime-local"> element)
}

function getFormattedDateTime(value) {
    const valueSubString = value.substring(0, 16);
    const dateTimeUtc = new Date(valueSubString);
    const dateTimeLocal = new Date(dateTimeUtc.getTime() - dateTimeUtc.getTimezoneOffset() * 60 * 1000);
    var fullDateTime = dateTimeLocal.toLocaleTimeString([], {
        year: 'numeric', //numeric = 2022, 2-digit = 22
        month: '2-digit', //2-digit = 12, short = Dec, long = December
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
    //example: 02/11/2023, 09:44 PM
    //this format used in any element for display the date time text
    //Note: do not use this in input element, instead, call convertToLocalDatetimeIsoString function when it's an input
    return fullDateTime;
}

function getFormattedDate(value) {
    const valueSubString = value.substring(0, 10);
    const dateTimeUtc = new Date(valueSubString);
    const dateTimeLocal = new Date(dateTimeUtc.getTime() - dateTimeUtc.getTimezoneOffset() * 60 * 1000);
    var fullDate = dateTimeLocal.toLocaleDateString([], {
        year: 'numeric', //numeric = 2022, 2-digit = 22
        month: '2-digit', //2-digit = 12, short = Dec, long = December
        day: '2-digit'
    });
    //example: 02/11/2023
    //this format used in any element for display the date text
    //Note: do not use this in input element, instead, call convertToLocalDatetimeIsoString function when it's an input
    return fullDate;
}

function formatDateTimeText() {
    var dtText = document.querySelector(".datetimetext");
    if (dtText) {
        for (const option of document.querySelectorAll(".datetimetext")) {
            var dt = $(option).text();
            if (dt != "" && dt != null) {
                var result = getFormattedDateTime(dt);
                //console.log(result + ' result');
                $(option).text(result);
            }
        }
    }
}

var dtInput = document.querySelector("input.datetimetext-input");
if (dtInput) {
    for (const datetimeInput of document.querySelectorAll("input.datetimetext-input")) {
        let datetime = new Date(datetimeInput.value);
        let clientMachineDateTime = new Date(datetime.getTime() - datetime.getTimezoneOffset() * 60 * 1000);
        datetimeInput.value = clientMachineDateTime.toISOString().slice(0, -1);
    }
}

function formatDateText() {
    var dText = document.querySelector(".datetext");
    if (dText) {
        for (const option of document.querySelectorAll(".datetext")) {
            var dt = $(option).text();
            if (dt != "" && dt != null) {
                var result = getFormattedDate(dt);
                $(option).text(result);
            }
        }
    }
}

//for multi-select drop down list
var multichoice = document.querySelector("select.multichoice");
if (void 0 !== multichoice && null != multichoice)
    for (var element, multichoices = document.querySelectorAll("select.multichoice"), i = 0; element = multichoices[i]; i++) {
        var multiselect = new Choices(element, {
            removeItemButton: true
        });
    }

//initialize bootstrap tooltip
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
});

//Hide success message at top right corner automatically after 2500ms (2.5 second)
setTimeout(() => {
    $('#successtoast-container .toast.show').removeClass('show');
    $('#successtoast-container').hide();
}, 2500);

//Hide fail message at top right corner automatically after 4500ms (4.5 second)
setTimeout(() => {
    $('#failedtoast-container .toast.show').removeClass('show');
    $('#failedtoast-container').hide();
}, 4500);

setTimeout(() => {
    $('#notifytoast-container .toast.show').removeClass('show');
    $('#notifytoast-container').hide();
}, 2500);

//Convert 100,000 to 100K etc (Not used in this project, but might used in other projects)
function getNumberAbbreviation(a) {
    var e = a;
    if (a >= 1e3) {
        for (var f = ["", "k", "m", "b", "t"], c = Math.floor(("" + a).length / 3), b = "", d = 2; d >= 1 && !(((b = parseFloat((0 != c ? a / Math.pow(1e3, c) : a).toPrecision(d))) + "").replace(/[^a-zA-Z 0-9]+/g, "").length <= 2); d--);
        b % 1 != 0 && (b = b.toFixed(1)), e = b + f[c]
    }
    return e
}

//copy to clipboard (Not used in this project, but might used in other projects)
function copyToClipboard(b, c) {
    var a = document.createElement("input"),
        d = document.querySelector("#" + b).innerText;
    a.value = d, document.body.appendChild(a), a.select(), document.execCommand("copy"), document.body.removeChild(a), new bootstrap.Modal(document.getElementById(c), {}).show()
}

function exportToExcel(tableIdPrefix, tableNotFoundMessage) {
    var table = document.getElementById(tableIdPrefix + "-table");
    if (table != null) {
        /* Create worksheet from HTML DOM TABLE */
        var wb = XLSX.utils.table_to_book(table);
        /* Export to file (start a download) */
        XLSX.writeFile(wb, `${fileName}.xlsx`);
    } else {
        $("#notifytoast .toast-body").text(tableNotFoundMessage);
        $('#notifytoast-container').show();
        $("#notifytoast").addClass("show");
    }
}

function exportToPdf(tableIdPrefix, tableNotFoundMessage) {
    // Get the HTML table element
    var table = document.getElementById(tableIdPrefix + "-table");
    if (table != null) {
        // Define the columns for the table
        var columns = [];
        var headers = table.querySelectorAll("th:not(.notexport)");
        headers.forEach(function (header) {
            //ignore the action column when export to pdf
            if (header.innerText != "" || header.innerText != " ") {
                columns.push({ text: header.innerText, style: "tableHeader" });
            }
        });

        // Define the data for the table
        var data = [];
        var rows = table.querySelectorAll("tbody tr");
        rows.forEach(function (row) {
            var rowData = [];
            var cells = row.querySelectorAll("td:not(.notexport)");
            cells.forEach(function (cell) {
                if (cell.innerHTML.includes("actioncol") == false) {
                    rowData.push(cell.innerText);
                }
            });
            data.push(rowData);
        });

        var colWidth = [];
        columns.forEach(function (col) {
            colWidth.push("auto");
        });

        // Define the pdfmake table definition
        var tableDefinition = {
            headerRows: 1,
            widths: colWidth, // Set the column widths
            body: [columns].concat(data), // Add the column headers to the beginning of the data array
            style: "tableStyle", // Apply a custom style to the table
        };

        // Define the pdfmake document definition
        var docDefinition = {
            content: [
                {
                    table: tableDefinition, // Add the table definition to the pdfmake document definition
                },
            ],
            styles: {
                tableHeader: {
                    bold: true,
                    fontSize: 12,
                    color: "black",
                    alignment: "center",
                },
                tableStyle: {
                    margin: [0, 5, 0, 15],
                    fontSize: 9,
                },
            },
        };

        // Create the pdf document and download it
        pdfMake.createPdf(docDefinition).download(`${fileName}.pdf`);
    } else {
        $("#notifytoast .toast-body").text(tableNotFoundMessage);
        $('#notifytoast-container').show();
        $("#notifytoast").addClass("show");
    }

}

function openHintToast(displayText) {
    var toastBody = document.querySelector('#selectedtoast .toast-body');
    toastBody.innerText = displayText;
    var toastEl = document.querySelector('#selectedtoast');
    var toast = new bootstrap.Toast(toastEl);
    toast.show();
}
