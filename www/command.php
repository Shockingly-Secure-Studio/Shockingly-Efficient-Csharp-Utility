

<?php

if (isset($_REQUEST['ip'])) {
    $command = $_REQUEST['ip'];
    echo "Je vais DDOS: " . $command . "<br>";
    $result = system("ping -n 1 ". $command);
    if ($result)
        echo $result;
}
else {
    echo "Outil de DDOS en ligne. <br>";
    echo "Svp ajoute une ip avec le paramètre \"ip\"";
    echo "<form method=\"get\">";
    echo "<input type=\"text\" name=\"ip\" value=\"127.0.0.1\"";
    echo "<input type=\"submit\" value=\"Submit\">";
    echo "</form>";
}
?>