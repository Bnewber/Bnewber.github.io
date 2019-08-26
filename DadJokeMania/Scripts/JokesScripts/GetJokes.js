window.onscroll = function () { scrollFunction(); };

var timer = document.getElementById("progressBarDiv");
var shortJokeDisplay = document.getElementById("shortJokeDisplay");
var mediumJokeDisplay = document.getElementById("mediumJokeDisplay");
var longJokeDisplay = document.getElementById("longJokeDisplay");

var errorAlert = document.getElementById("errorAlert");
var nojokeAlert = document.getElementById("errorAlert");

var getJokeInterval = -1;
var randomJokeCount = 0;

document.getElementById("RandomJokeBtn").addEventListener("click", function () {
    var currButton = $(this);
    var nextPageBtn = $("#nextPageBtn");
    var previousPageBtn = $("#previousPageBtn");
    $("#searchTerms").val("");
    $(nextPageBtn).addClass("hidden disabled");
    $(previousPageBtn).addClass("hidden disabled");

    $(".progress").removeClass("hidden");
    clearAlerts();
    stopProgressBar(timer);
    clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay);
    getRandomJoke(currButton, shortJokeDisplay);

});

document.getElementById("SearchForJokeBtn").addEventListener("click", function () {
    var currButton = $(this);
    var searchTerms = $("#searchTerms");
    var nextPageBtn = $("#nextPageBtn");
    var previousPageBtn = $("#previousPageBtn");

    if (getJokeInterval !== -1) {//Stop fetching random jokes
        clearInterval(getJokeInterval);
    }

    stopProgressBar(timer);//stop and hide loading/timer bar
    $(".progress").addClass("hidden");

    clearAlerts();
    clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay);

    if (searchTerms.val() !== "") {
        $.ajax({
            method: "GET",
            url: currButton.data("url"),
            dataType: "json",
            data: { queryTerms: searchTerms.val(), page: currButton.data("page-number") },
            success: (data) => {

                if (data.success) {

                    if (data.jokes.length > 0) {
                        var shortJokeHtml = "";
                        var mediumJokeHtml = "";
                        var longJokeHtml = "";

                        data.jokes.forEach((joke) => {
                            var jokeWordCount = joke.joke.split(" ").length;

                            //Group jokes by length
                            if (jokeWordCount <= 10) {
                                shortJokeHtml += "<div class='well well-sm'><p>" + joke.joke + "</p></div>";
                            } else if (jokeWordCount > 10 && jokeWordCount <= 15) {
                                mediumJokeHtml += "<div class='well well-sm'><p>" + joke.joke + "</p></div>";
                            } else {
                                longJokeHtml += "<div class='well well-sm'><p>" + joke.joke + "</p></div>";
                            }

                        });


                        $(shortJokeDisplay).html(shortJokeHtml);
                        $(mediumJokeDisplay).html(mediumJokeHtml);
                        $(longJokeDisplay).html(longJokeHtml);

                        currButton.data("page-number", data.currentPageNumber);//Need to keep track of the current page number so we can enable or disable the pagination


                        $(nextPageBtn).removeClass("hidden");
                        $(previousPageBtn).removeClass("hidden");

                        $(nextPageBtn).data("next-page-number", data.nextPageNumber);
                        $(previousPageBtn).data("previous-page-number", data.previousPageNumber);

                        console.log(data);
                        if (data.currentPageNumber < data.totalPages) {
                            $(nextPageBtn).removeClass("disabled").removeAttr("disabled");
                        } else {
                            $(nextPageBtn).addClass("disabled").attr("disabled", true);;
                        }

                        if (data.currentPageNumber > 1) {
                            $(previousPageBtn).removeClass("disabled").removeAttr("disabled");

                        } else {
                            $(previousPageBtn).addClass("disabled").attr("disabled", true);
                        }
                    } else {
                        $(noJokeAlert).removeClass("hidden").html("No jokes found! Please try another word.");
                    }
                } else {
                    $(errorAlert).removeClass("hidden").html(data.responseText);
                }
            },
            error: () => {
                $(errorAlert).removeClass("hidden").html("<p>Aww Snap something went wrong, please try again.</p>");
            }
        });
    } else {
        $(errorAlert).removeClass("hidden").html("Please enter a value in the search box.");
    }
});

document.getElementById('searchTerms').addEventListener("keypress", (e) => {
    if (e.which === 13)  // the enter key code
    {
        e.preventDefault();
        $('#SearchForJokeBtn').click();
        return false;
    }
});

document.getElementById('moreJokesBtn').addEventListener("click", () => {
    if (getJokeInterval !== -1) {//Stop fetching random jokes
        clearInterval(getJokeInterval);
    }
    clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay);
    randomJokeCount = 0;
    $("#RandomJokeBtn").click();
});

document.getElementById('noMoreJokesBtn').addEventListener("click", () => {
    if (getJokeInterval !== -1) {//Stop fetching random jokes
        clearInterval(getJokeInterval);
    }
    randomJokeCount = 0;
    clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay);
    $("#searchTerms").val("");
});

document.getElementById('searchMoreJokesBtn').addEventListener("click", () => {
    if (getJokeInterval !== -1) {//Stop fetching random jokes
        clearInterval(getJokeInterval);
    }
    randomJokeCount = 0;
    clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay);
    $("#searchTerms").val("");
    $("#searchTerms").focus();
});

document.getElementById('nextPageBtn').addEventListener("click", () => {

    var searchButton = $("#SearchForJokeBtn");
    var currentPageNumber = searchButton.data("page-number");
    searchButton.data("page-number", currentPageNumber + 1);
    console.log(currentPageNumber);
    $(searchButton).click();
});

document.getElementById('previousPageBtn').addEventListener("click", () => {

    var searchButton = $("#SearchForJokeBtn");
    var currentPageNumber = searchButton.data("page-number");
    searchButton.data("page-number", currentPageNumber - 1);
    $(searchButton).click();
});

function getRandomJoke(currButton, shortJokeDisplay) {

    if (checkIfUserIsStillReadingJokes()) {
        $.ajax({
            method: "GET",
            url: currButton.data("url"),
            dataType: "json",
            success: (data) => {

                if (data.success) {
                    var shortJokeHtml = "";
                    shortJokeHtml += "<div class='well well-sm'><p>" + data.joke.joke + "</p></div>";
                    $(shortJokeDisplay).html(shortJokeHtml);

                    stopProgressBar(timer);
                    $(timer).animate({
                        width: "100%"
                    }, 10000);

                    //check if we are already have a timer to fetch a new joke and stop it, 
                    //so that we can fetch a new one immediatly and not run into issues with multiple timers running
                    if (getJokeInterval !== -1) {
                        clearInterval(getJokeInterval);
                        getJokeInterval = startTimer(currButton, timer);
                    } else {
                        getJokeInterval = startTimer(currButton, timer);
                    }
                } else {
                    $(errorAlert).removeClass("hidden").html(data.responseText);
                }

            },
            error: () => {
                $(errorAlert).removeClass("hidden").html("<p>Aww Snap something went wrong, please try again.</p>");
            }
        });
        randomJokeCount++;
    }
}

function startTimer(currButton, timer) {
    return setInterval(() => {
        getRandomJoke(currButton, shortJokeDisplay);
    }, 10000);
}

function stopProgressBar(timer) {
    $(timer).animate().stop();
    $(timer).css("width", '0%');
}

function clearJokeDisplays(shortJokeDisplay, mediumJokeDisplay, longJokeDisplay) {
    $(shortJokeDisplay).html("");
    $(mediumJokeDisplay).html("");
    $(longJokeDisplay).html("");
}

function clearAlerts() {
    if ($(errorAlert).hasClass("hidden").valueOf() === false) { $(errorAlert).addClass("hidden"); }
    if ($(noJokeAlert).hasClass("hidden").valueOf() === false) { $(noJokeAlert).addClass("hidden"); }
}

function checkIfUserIsStillReadingJokes() {
    if (randomJokeCount === 20) {

        if (getJokeInterval !== -1) {//Stop fetching random jokes
            clearInterval(getJokeInterval);
        }
        stopProgressBar(timer);//stop and hide loading/timer bar
        $(".progress").addClass("hidden");
        $("#twentyJokesAlert").removeClass("hidden").slideDown(5000);
        return false;
    }
    return true;
}

function scrollFunction() {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        document.getElementById("previousPageBtn").style.display = "block";
        document.getElementById("nextPageBtn").style.display = "block";
    } else {
        document.getElementById("previousPageBtn").style.display = "none";
        document.getElementById("nextPageBtn").style.display = "none";
    }
}

function topFunction() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}