$(() => {
    $('#btn-search').click(() => {
        $('#result').text('-');
        const url = $('#search').data('url').replace('%%', $('#input-city').val());
        console.log(url);
        $.get(url,
            (data) => {
                $('#result').text(`${data} °C`);
            });
    });
});