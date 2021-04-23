var idInput;
var showImagesBtn;
var removeImagesBtn;
var imageCointainer;

var allImages = [];

function preload() {
}

function setup() {
  idInput = select('#idInput');
  showImagesBtn = select('#showImagesBtn');
  removeImagesBtn = select('#removeImagesBtn');
  imageCointainer = select('#imageCointainer');

  showImagesBtn.mousePressed(loadImages);
  removeImagesBtn.mousePressed(removeImages);

  removeImagesBtn.style("display", "none");
}

function draw() {
  
}

function loadImages() {
  var sessionId = idInput.value();
  var jsFileName = "images/" + sessionId + ".txt";
  loadStrings(jsFileName, showImages, wrongSessingId);
  print(jsFileName);
}

function wrongSessingId() {
  alert("No images were saved for this session id. Check if you have mistyped.");
}

function showImages(imageIds) {

  for (var i = 0; i < imageIds.length; i++) {
    if(imageIds[i] != "") {
      //var imgUrl = "https://drive.google.com/uc?id=" + imageIds[i];
      var imgUrl = "https://drive.google.com/uc?export=view&id=" + imageIds[i];
      var img = createImg(imgUrl,"Image " + i);
      img.addClass("genImage");
      img.parent("imageCointainer");
      allImages[i] = img;
    }
  }
  showImagesBtn.style("display", "none");
  idInput.style("display", "none");
  removeImagesBtn.style("display", "block");
}

function removeImages() {
  for (var i = 0; i < allImages.length; i++) {
    allImages[i].remove();
  }
  allImages = [];

  showImagesBtn.style("display", "block");
  idInput.style("display", "block");
  removeImagesBtn.style("display", "none");
}


function keyPressed() {

}

