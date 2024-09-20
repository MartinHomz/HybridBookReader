let book = ePub();
let rendition;
let _blazorReference;

export function registerBlazorReference(dotNetHelper) {
    _blazorReference = dotNetHelper;
}

export function initReader(binary, lastSavedPosition, fontSize, theme) {
    //#region Načtení knihy
    var bookData = binary;

    book.open(bookData, "binary");

    rendition = book.renderTo("viewer", {
        width: "100%",
        height: "100%"
    });

    let displayed = rendition.display();

    rendition.on("relocated", function (location) {
        _blazorReference.invokeMethodAsync("LocationChanged",
            location.start.cfi, parseInt(book.locations.total),
            parseInt(book.locations.locationFromCfi(location.start.cfi)));
    });

    //#endregion Načtení knihy

    //Načtení stránek
    book.ready.then(function () {
        book.locations.generate(150);
        rendition.display(lastSavedPosition);
    });

    //Výběr
    rendition.on("selected", function (cfiRange) {
        book.getRange(cfiRange).then(function (range) {
            if (range)
                _blazorReference.invokeMethodAsync("ShowWordDictionary", range.toString());
        })
    });

    //Témata
    rendition.themes.register("FgBlackBgWhite", "css/style.min.css");
    rendition.themes.register("FgRedBgWhite", "css/style.min.css");
    rendition.themes.register("FgBlueBgWhite", "css/style.min.css");
    rendition.themes.register("FgBlueBgBlack", "css/style.min.css");
    rendition.themes.register("FgWhiteBgBlack", "css/style.min.css");
    rendition.themes.select(theme);
    rendition.themes.fontSize(fontSize);

    //#region Ovládání
    var next = document.getElementById("next");
    var prev = document.getElementById("prev");

    let keyListener = function (e) {

        // Left Key
        if ((e.keyCode || e.which) == 37) {
            rendition.prev();
        }

        // Right Key
        if ((e.keyCode || e.which) == 39) {
            rendition.next();
        }

    };

    rendition.on("keyup", keyListener);

    next.addEventListener("click", function (e) {
        rendition.next();
        e.preventDefault();
    }, false);

    prev.addEventListener("click", function (e) {
        rendition.prev();
        e.preventDefault();
    }, false);

    document.addEventListener("keyup", keyListener, false);
    //#endregion Ovládání a pozice
}

export function setFontSize(fontSize) {
    rendition.themes.fontSize(fontSize);
}

export function setColorTheme(theme) {
    rendition.themes.select(theme);
}

export function changePosition(cfiPosition) {
    rendition.display(cfiPosition);
}