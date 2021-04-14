onmessage = function (e) {
    let duration = e.data[0];
    let width = 100;
    setTimeout(onTick, 10);

    function onTick() {
        if (width <= 0) {
            postMessage(['finished']);
        } else {
            setTimeout(onTick, 10);
            width -= 1 / duration;
            postMessage(['update', width])
        }
    }
}