<style>
  table, th, td {
    border: 1px solid black;
  }
  </style>
<a href="command.php">DDOS</a>
<form method="get">";
<input type="text" name="name" value="table">
<input type="submit" value="Submit">
</form>
<?php

$db = new SQLite3("database.db");

if (isset($_GET['name'])) {
	$res = $db->query("SELECT * FROM products WHERE name LIKE \"%". $_GET["name"] ."%\"");

	echo "<table style=\"width:100%\">\n";
	echo "<tr>\n";
	echo "<th> Name </th><th> Description </th><th> Quantity </th>\n";
	echo "</tr>\n";
	while ($values = $res->fetchArray()) {
		echo "<tr>\n";
		echo "<th> ". $values["name"] ." </th><th> ". $values["description"] ." </th><th> ". $values["quantity"] . "</th>\n";
		echo "</tr>\n";
	}
}



?>