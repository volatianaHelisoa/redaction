
/*cette fonction sert a detecter le chargement d'une page
lors d'un chargement, on veu savoir si le fichier en cours est "" ou Index pour cacher le menu en haut
*/
$(window).on('load', function () {
    //var CheminRepertoire = CheminComplet.substring(0, CheminComplet.lastIndexOf("/"));
    var CheminComplet = document.location.href;
    var nomDuFichier = CheminComplet.substring(CheminComplet.lastIndexOf("/") + 1);
    if (nomDuFichier == "" || nomDuFichier == "Index" || nomDuFichier == "MotDePasseOublie" ||
        nomDuFichier == "ReussiEnvoiMail" || nomDuFichier == "ModifierMotDePasse" || nomDuFichier.indexOf("ModifierMotDePasse")!==-1) {      
        console.log("passage dans l'authentification");
        $(".top-panel").hide();
    }
    else {
        $(".top-panel").show();       
    }
   
});