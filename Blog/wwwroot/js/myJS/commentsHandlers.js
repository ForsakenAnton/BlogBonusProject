
async function mainCommentHandler(event) {

    event.preventDefault();

    let form = document.forms.mainCommentForm;
    let message = form.elements.Message.value;
    let postId = form.elements.postId.value;

    const response = await fetch("/Comments/CreateComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            message: message,
            postId: postId
        })
    });

    if (response) {

        recountComments(1);

        const writeParentCommentAndChildrenPartial = await response.text();
        document.getElementById("mainCommentId").innerHTML += writeParentCommentAndChildrenPartial;
    }
    else {
        console.log(response.statusText);
    }
}


async function replyCommentHandler(event, commentId) {

    event.preventDefault();

    console.log(commentId);
    let formName = "replyCommentForm".concat(commentId);

    let form = document.forms[formName]; //.replyCommentForm;
    let message = form.elements.message.value;

    // Примитивная валидация
    if (isNullOrWhitespace(message)) {
        document.getElementById("validationReply".concat(commentId))
            .innerText = "Comment's text is required!";
        
        return;
    }
    else {
        document.getElementById("validationReply".concat(commentId))
            .innerText = "";
    }
    // конец примитивной валидации

    let postId = form.elements.postId.value;
    let parentCommentId = form.elements.parentCommentId.value;
    let currentNested = form.elements.currentNested.value;
    let isReply = true;

    const response = await fetch("/Comments/CreateComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            message: message,
            postId: postId,
            parentCommentId: parentCommentId,
            currentNested: currentNested,
            isReply: isReply
        })
    });

    // console.log(event.target.parentNode.parentNode.parentNode);

    if (response) {

        recountComments(1);

        let parentBlock = event.target.parentNode.parentNode.parentNode;
        const writeParentCommentAndChildrenPartial = await response.text();
        parentBlock.innerHTML += writeParentCommentAndChildrenPartial;

        // скрываем поле ввода
        const collapseReplyForm = document
            .getElementById("collapseReplyForm".concat(commentId));

        collapseElement(collapseReplyForm);
        //
    }
    else {
        console.log(response.statusText);
    }
}


async function editCommentHandler(event, commentId) {

    event.preventDefault();

    console.log(commentId);
    let formName = "editCommentForm".concat(commentId);

    let form = document.forms[formName]; //.editCommentForm;
    let message = form.elements.message.value;

    // Примитивная валидация
    if (isNullOrWhitespace(message)) {
        document.getElementById("validationEdit".concat(commentId))
            .innerText = "Comment's text is required!";

        return;
    }
    else {
        document.getElementById("validationEdit".concat(commentId))
            .innerText = "";
    }
    // конец примитивной валидации

    const response = await fetch("/Comments/EditComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            id: commentId,
            message: message,
        })
    });

    // console.log(event.target.parentNode.parentNode.parentNode);

    if (response.ok) {
        document.getElementById("messageId".concat(commentId)).innerText = message;

        // скрываем поле ввода
        const collapseEditForm = document
            .getElementById("collapseEditForm".concat(commentId));

        collapseElement(collapseEditForm);
        //
    }
    else {
        console.log(response.statusText);

        document.getElementById("validationEdit".concat(commentId))
            .innerText = "Мessage editing error! Please, try again later.";
    }
}


async function deleteCommentHandler(event, commentId) {

    event.preventDefault();

    const response = await fetch("/Comments/DeleteComment", {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify(
            commentId
        )
    });

    if (response.ok) {

        //заклинанием ниже преобразуем текст ответа в минусовое число 
        recountComments(-Number(await response.text()));

        let parentBlock = event.target.parentNode.parentNode.parentNode;
        //parentBlock.remove();
        parentBlock.classList.add("removedTransform");

        parentBlock.addEventListener("transitionend", () => {
            parentBlock.className = ""; //remove();
            parentBlock.innerHTML = `
                <h3 class='bg-danger rounded rounded-3 ps-3'>
                    Comment Was Deleted
                </h3>`;
        });
    }
    else {
        console.log(response);
    }
}
/////////////////////////////////////////


function collapseReplyForm(commentId) {

    const collapseEditForm = document
        .getElementById("collapseEditForm".concat(commentId));

    collapseElement(collapseEditForm);
}

function copyTextOfCommentToEditForm(textMessage, commentId) {

    document.getElementById("editTextAreaId".concat(commentId)).innerText = textMessage;

    const collapseReplyForm = document
        .getElementById("collapseReplyForm".concat(commentId));

    collapseElement(collapseReplyForm);
}

function collapseElement(element) {

    if (element) {
        const bsElement = new bootstrap.Collapse(element, {
            toggle: false,
        });

        bsElement.hide();
    }
}

function isNullOrWhitespace(input) {

    if (typeof input === 'undefined' || input == null) {
        return true;
    }

    return input.replace(/\s/g, '').length < 1;
}


function recountComments(count) {

    let commentsCountElement = document.getElementById("commentsCountId");
    let commentsCount = commentsCountElement.innerText;
    commentsCount = +commentsCount + count;
    commentsCountElement.innerText = commentsCount;
}