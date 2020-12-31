var postService = function () {

    const baseUrl = window.location.origin;

    function like(postId) {
        const likeUrl = `${baseUrl}/api/post/${postId}/like`;

        return fetch(likeUrl, {
            method: "POST",
        }).then(response => response.json());
    }

    function cancelLike(postId) {
        const likeUrl = `${baseUrl}/api/post/${postId}/like`;

        return fetch(likeUrl, {
            method: "DELETE",
        }).then(response => response.json());
    }

    return {
        like: like,
        cancelLike: cancelLike
    }
}();

var postController = function (postService) {

    let likeButtons = null;

    function initialize() {
        likeButtons = document.querySelectorAll(".js-like");
        likeButtons.forEach(button => button.addEventListener("click", like));
    }

    function getPostIdAttribute(e) {
        return e.target.dataset.postId;
    }

    function hasUserLikedPostAttribute(e) {
        return e.target.dataset.liked === "True";
    }

    function toggleLikeButton(likeEl, removedLike) {
        // 1. Update button data attribute (data-liked="<boolean>")
        // 2. Update button class e.g liked = "btn btn-primary" removed like = "btn btn-light"
        // 3. Update button text

        likeEl.dataset.liked = removedLike ? "False" : "True";

        if (removedLike) {
            likeEl.classList.remove("btn-primary");
            likeEl.classList.add("btn-light");
            likeEl.innerText = "Like";
        } else {
            likeEl.classList.remove("btn-light");
            likeEl.classList.add("btn-primary");
            likeEl.innerText = "Liked";
        }
    }


    function updateLikeBadge(postId, likeCount) {
        const likeBadgeEl = document.querySelector(`.js-like-count[data-post-id="${postId}"]`);

        likeBadgeEl.dataset.likeCount = likeCount;
        likeBadgeEl.innerHTML = `${likeCount} ${likeCount == 1 ? "Like" : "Likes"}`;
    }

    function like(e) {
        const likeEl = e.target;
        const postId = getPostIdAttribute(e);
        const cancelLike = hasUserLikedPostAttribute(e);

        if (cancelLike) {
            postService.cancelLike(postId).then(data => {
                toggleLikeButton(likeEl, cancelLike);
                updateLikeBadge(postId, data.postLikes);
            });
        } else {
            postService.like(postId).then(data => {
                toggleLikeButton(likeEl, cancelLike);
                updateLikeBadge(postId, data.postLikes);
            });
        }
        
    }

    return {
        initialize: initialize
    }
}(postService);


document.addEventListener("DOMContentLoaded", function () {
    postController.initialize();
})