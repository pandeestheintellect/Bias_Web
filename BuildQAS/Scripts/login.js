$(document).ready(function(){
	$("#loginForm").validate( {
        rules: {
            Password: {
                required: true,
                minlength: 5
            },
            UserName: {
                required: true
            }
        },
        messages: {
            Password: {
                required: "Please provide a password",
                minlength: "Your password must be at least 5 characters long"
            },
            UserName: "Please enter a valid user name"
        },
        //errorElement: "em",
        //errorPlacement: function ( error, element ) {
        //    // Add the `invalid-feedback` class to the error element
        //    error.addClass( "invalid-feedback" );
        //    if ( element.prop( "type" ) === "checkbox" ) {
        //        error.insertAfter( element.next( "label" ) );
        //    } else {
        //        error.insertAfter( element );
        //    }
        //},
        highlight: function ( element, errorClass, validClass ) {
            $( element ).addClass( "is-invalid" ).removeClass( "is-valid" );
        },
        unhighlight: function (element, errorClass, validClass) {
            $( element ).addClass( "is-valid" ).removeClass( "is-invalid" );
        }
    } );
});