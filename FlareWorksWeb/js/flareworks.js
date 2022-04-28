// Reset a user's password
function reset_password(id, name) {
    var input_box = confirm("Do you really want to reset the password for '" + name + "'?");
    if (input_box == true) {
        // Set the hidden value this data
        var hiddenfield = document.getElementById('admin_user_id');
        hiddenfield.value = id;

        var hiddenfield2 = document.getElementById('admin_user_action');
        hiddenfield2.value = 'reset';

        // Submit this
        document.getElementById('form1').submit();
    }

    // Return false to prevent another return trip to the server
    return false;
}


function numbers_only(e) {

    e = (e) ? e : window.event;

    // Allow: backspace, delete, tab, escape, and enter
    if ((e.keyCode == 8) || (e.keyCode == 9) || (e.keyCode == 27) || (e.keyCode == 13) || (e.keyCode == 190))
        return true;


    // Allow: Ctrl+A, Ctrol+C, Ctrl+V, Command+A
    if ((e.keyCode == 65 || e.keyCode == 86 || e.keyCode == 67) && (e.ctrlKey === true || e.metaKey === true))
        return true;

    // Allow: home, end, left, right, down, up
    if (e.keyCode >= 35 && e.keyCode <= 40)
        return true;

    // Ensure that it is a number and stop the keypress
    if ((e.keyCode < 48 || e.keyCode > 57) && ( e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
        return false;
    }

    

    return true;
}


function UserDeleteConfirmation() {
    return confirm("Are you sure you want to delete this row?");
}

function ApproveAllConfirmation() {
    return confirm("Are you sure you want to approve this work?");
}