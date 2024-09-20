var book = ePub();
var rendition;

var inputElement = document.getElementById("input");

inputElement.addEventListener('change', function (e) {
      var file = e.target.files[0];
      if (window.FileReader) {
          var reader = new FileReader();
          reader.onload = openBook;
          reader.readAsArrayBuffer(file);
      }
  });

function openBook(e){
  var bookData = e.target.result;
  var title = document.getElementById("title");
  var next = document.getElementById("next");
  var prev = document.getElementById("prev");

  book.open(bookData, "binary");

  rendition = book.renderTo("viewer", {
    width: "100%",
    height: 600
  });

  rendition.display();
  
  var keyListener = function(e){

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
  rendition.on("relocated", function(location){
    console.log(location);
  });

  next.addEventListener("click", function(e){
    rendition.next();
    e.preventDefault();
  }, false);

  prev.addEventListener("click", function(e){
    rendition.prev();
    e.preventDefault();
  }, false);

  


  document.addEventListener("keyup", keyListener, false);
}