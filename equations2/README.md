
###Grammer:
expr ::= term((ADD|MINUS)term)*

term ::= factor((MULTIPLY|DIVIDE)factor)*

factor ::= INTEGER|LPARAM expr RPARAM


####expr

![expr](https://cloud.githubusercontent.com/assets/234688/11408707/c7ab755a-936f-11e5-9e65-c7046fd498e8.png)


####term

![term](https://cloud.githubusercontent.com/assets/234688/11408716/d154af0e-936f-11e5-9290-5023ab6f018d.png)


####factor

![factor](https://cloud.githubusercontent.com/assets/234688/11408715/cea0e4d0-936f-11e5-8f2f-a0740a913015.png)


