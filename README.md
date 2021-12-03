# matrix-filters

Interfejs programu w większości wydaje mi się być na tyle intuicyjny,
by nie było problemów z jego obsługą.
Aby nałożyć filtr na obszar określony wielokątem należy nacisnąć radio button Polygon.
Można wtedy na obrazku klikając wybrać jego wierzchołki.
Aby zmodyfikować parametry predefiniowanego filtra macierzowego,
należy go wybrać, a następnie wybrać Custom. Poprzednio wybrany filtr nie zniknie,
tylko na jego bazie będzie można tworzyć nowy i zmieniać jego parametry.
Ustawić można też dowolne wymiary nakładanego filtra.
Jeśli macierz filtra wykracza poza obrazek
(np. będziemy próbować nałożyć filtr o dużej macierzy blisko krawędzi obrazka)
to jako wartości brakujących pikseli będą brane wartości pikseli brzegowych.
