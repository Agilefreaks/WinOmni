namespace OmniHolidays.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using OmniCommon.Interfaces;

    public class GreetingDefinitionProvider : IMessageDefinitionProvider
    {
        private readonly IConfigurationService _configurationService;

        private List<MessageDefinition> _messageCache;

        public GreetingDefinitionProvider(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public IObservable<IList<MessageDefinition>> Get()
        {
            if (_messageCache != null)
            {
                Observable.Return(_messageCache);
            }

            const string Family = "Family";
            const string Friends = "Friends";
            const string Colleagues = "Colleagues";
            const string School = "School";

            const string English = "English";
            const string Romanian = "Romanian";
            const string Portuguese = "Portuguese";
            const string Polish = "Polish";

            _messageCache = new List<MessageDefinition>
                               {
                                   #region Portuguese

                                   new MessageDefinition(Portuguese, Family, "Olá %ContactFirstName%, Gostaria de te desejar um Feliz Natal."),
                                   new MessageDefinition(Portuguese, Family, "Olá %ContactFirstName%, Desejo-te umas Boas Festas, com muitos doces e prendas! \r\nP.S: Deixa algumas para mim!"),
                                   new MessageDefinition(Portuguese, Family, "De: %UserFirstName%,\r\nPara: %ContactFirstName%\r\nEscrevo esta mensagem para te desejar Boas Festas!"),
                                   new MessageDefinition(Portuguese, Family, "Uma vida sem amor seria solidão, uma vida sem ternura seria cruel, uma vida sem confiança seria vazia, mas sem a família não seria vida! Feliz Natal!"),
                                   new MessageDefinition(Portuguese, Family, "Olá %ContactFirstName%, Obrigado por fazeres parte da minha vida. Desejo-te um Feliz Natal!"),
                                   
                                   new MessageDefinition(Portuguese, Friends, "Olá %ContactFirstName%, Que tenhas um Feliz Natal junto dos que mais gostas, com muita saúde!"),
                                   new MessageDefinition(Portuguese, Friends, "Sabes qual é a bebida favorita do Pai Natal?\r\nR: O Gin-Gobel.\r\nFeliz Natal!"),
                                   new MessageDefinition(Portuguese, Friends, "Pai Natal, por acaso rói as unhas?\r\nR: Rou, rou, rou...\r\nBoas Festas!"),
                                   new MessageDefinition(Portuguese, Friends, "Meu amigo %ContactFirstName%,\r\nQuero desejar-te um Feliz Natal, com muitos doces e prendas!\r\nMas cuidado, não comas tudo! :)"),
                                   new MessageDefinition(Portuguese, Friends, "Olá %ContactFirstName%\r\nEste Natal queria dar-te uma prenda linda, fantástica, imprescindível, maravilhosa, autentica... mas infelizmente... eu não estou à venda!\r\nFeliz Natal!"),
                                   new MessageDefinition(Portuguese, Friends, "Olá %ContactFirstName%\r\nJá que nesta época se fala em sentimentos nobres, nada mais adequado que valorizar a amizade! Uma mensagem, não vale muito, mas a lembrança vale tudo! Feliz Natal!"),
                                   new MessageDefinition(Portuguese, Friends, "A hora passa a noite escurece o dia clareia a natureza cresce... mas um verdadeiro amigo, esse permanece nunca se esquece. Feliz Natal!"),

                                   new MessageDefinition(Portuguese, Colleagues, "Olá Chefe,\r\nDesejo-lhe votos de Boas Festas!"),
                                   new MessageDefinition(Portuguese, Colleagues, "Chefe, um Feliz Natal para si e para toda a sua família!"),
                                   new MessageDefinition(Portuguese, Colleagues, "Olá %ContactFirstName%,\r\nAproveita enquanto estamos de folga! Feliz Natal e tudo de bom!"),
                                   new MessageDefinition(Portuguese, Colleagues, "Resolvi interromper as minhas mini-férias para te desejar um Feliz Natal!"),

                                   new MessageDefinition(Portuguese, School, "Olá %ContactFirstName%,\r\nUm Feliz Natal para si, cheio de boas notas!"),
                                   new MessageDefinition(Portuguese, School, "Olá %ContactFirstName%,\r\nQue tenha um Feliz Natal junto dos que mais ama!"),
                                   new MessageDefinition(Portuguese, School, "Olá %ContactFirstName%,\r\nFeliz Natal, muitos doces, muitas prendas e boas Férias!"),
                                   new MessageDefinition(Portuguese, School, "Olá %ContactFirstName%,\r\nAproveita enquanto estamos de férias! Feliz Natal!"),
                                   new MessageDefinition(Portuguese, School, "Olá %ContactFirstName%,\r\nNão estudes mais, é Natal! Chegou a hora dos doces, de aquecer os pés e de abrir muitas prendas. Feliz Natal!"),
                                   
                                   #endregion

                                   #region Polish

                                   new MessageDefinition(Polish, Family, "Święta Bożego Narodzenia to czas błogosławiony. W tym uroczystym okresie, życzę wszystkiego najlepszego, spokoju, i miłości. Pozdrawiam, %UserFirstName%"),
                                   new MessageDefinition(Polish, Family, "Z okazji Świąt Bożego Narodzenia, szczęścia, miłości, i spokoju życzy %UserFirstName%"),
                                   new MessageDefinition(Polish, Family, "Oby Wam się wiodło w te Święta i w Nowym Roku. Wszystkiego dobrego, %UserFirstName%"),
                                   new MessageDefinition(Polish, Family, "Znowu Święta, znowu czas pysznego jedzenia i odpoczynku w rodzinnym gronie. W tym błogosławionym okresie pokoju i miłości Waszemu domowi życzy %UserFirstName%"),
                                   
                                   new MessageDefinition(Polish, Friends, "Hej, %ContactFirstName%! Wszystkiego dobrego w te Święta. No i oczywiście sukcesów w Nowym Roku! :)"),
                                   new MessageDefinition(Polish, Friends, "Cześć %ContactFirstName%, odpocznij wreszcie w te Święta i Nowy Rok zacznij z nowymi siłami. :)"),
                                   new MessageDefinition(Polish, Friends, "%ContactFirstName%, w te Święta życzę Ci dużo wypoczynku, dużo serdeczności, i dużo dobrego jedzenia. I dobrej zabawy w Sylwestra. ;)"),
                                   new MessageDefinition(Polish, Friends, "Znowu Święta... Nareszcie! :) %ContactFirstName%, baw się dobrze, wypocznij, i zacznij Nowy Rok pełną parą."),
                                   new MessageDefinition(Polish, Friends, "%ContactFirstName%, miłości i spokoju w te Święta, dobrej zabawy w Sylwestra, i sukcesów w Nowym Roku. Cóż więcej mogę Ci życzyć? :D"),
                                   new MessageDefinition(Polish, Friends, "Cześć, %ContactFirstName%. Przyszły Święta, więc życzę Ci spokoju, radości, i dobrego jedzenia. A po Świętach dobrej zabawy na Sylwestrze. Pozdrowienia! :)"),
                                   
                                   new MessageDefinition(Polish, Colleagues, "Spokojnych i wesołych Świąt Bożego Narodzenia i Szczęśliwego Nowego Roku życzy %ContactFirstName% %ContactLastName%"),
                                   new MessageDefinition(Polish, Colleagues, "Dużo miłości i pokoju w te Święta, oraz wszystkiego co dobre w Nowym Roku życzy %ContactFirstName% %ContactLastName%"),
                                   new MessageDefinition(Polish, Colleagues, "W te Święta Bożego Narodzenia odpoczynku w domowym zaciszu, pokoju i miłości życzy %ContactFirstName% %ContactLastName%"),
                                   new MessageDefinition(Polish, Colleagues, "Radosnych is zdrowych Świąt Bożego Narodzenia oraz pomyślności w Nowym Roku życzy %ContactFirstName% %ContactLastName%"),
                                   
                                   new MessageDefinition(Polish, School, "Hej, %ContactFirstName%! Wszystkiego dobrego w te Święta. No i oczywiście sukcesów w Nowym Roku! :)"),
                                   new MessageDefinition(Polish, School, "Cześć %ContactFirstName%, odpocznij wreszcie w te Święta i Nowy Rok zacznij z nowymi siłami. :)"),
                                   new MessageDefinition(Polish, School, "%ContactFirstName%, w te Święta życzę Ci dużo wypoczynku, dużo serdeczności, i dużo dobrego jedzenia. I dobrej zabawy w Sylwestra. ;)"),
                                   new MessageDefinition(Polish, School, "Znowu Święta... Nareszcie! :) %ContactFirstName%, baw się dobrze, wypocznij, i zacznij Nowy Rok pełną parą."),
                                   new MessageDefinition(Polish, School, "%ContactFirstName%, miłości i spokoju w te Święta, dobrej zabawy w Sylwestra, i sukcesów w Nowym Roku. Cóż więcej mogę Ci życzyć? :D"),
                                   new MessageDefinition(Polish, School, "Cześć, %ContactFirstName%. Przyszły Święta, więc życzę Ci spokoju, radości, i dobrego jedzenia. A po Świętach dobrej zabawy na Sylwestrze. Pozdrowienia! :)"),

                                   #endregion

                                   #region English

                                   new MessageDefinition(English, School, "Merry Christmas, %ContactFirstName%! I wouldn't have nearly as much fun as I have in school without you, so thank you for that. Happy holidays!"),
                                   new MessageDefinition(English, School, "Merry Christmas, %ContactFirstName%! Enjoy the vacation as much as you can and try not to think about school starting again in January! Happy holidays!"),
                                   new MessageDefinition(English, School, "Merry Christmas, Mr. %ContactFirstName%! Thank you for your patience and hard work in teaching us lessons, of school and also life. Happy holidays!"),
                                   new MessageDefinition(English, School, "Merry Christmas, Ms. %ContactFirstName%! Thank you for your patience and hard work in teaching us lessons, of school and also life. Happy holidays!"),
                                   new MessageDefinition(English, School, "Merry Christmas, Mr. %ContactFirstName%! I wish you many generations of students to help and be loved by. I'm thankful for having you as my teacher!"),
                                   new MessageDefinition(English, School, "Merry Christmas, Ms. %ContactFirstName%! I wish you many generations of students to help and be loved by. I'm thankful for having you as my teacher!"),

                                   new MessageDefinition(English, Colleagues, "Mr. %ContactFirstName%, thank you for being an amazing boss and helping us achieve the set goals. I hope you and your loved ones have a merry Christmas!"),
                                   new MessageDefinition(English, Colleagues, "Ms. %ContactFirstName%, thank you for being an amazing boss and helping us achieve the set goals. I hope you and your loved ones have a merry Christmas!"),
                                   new MessageDefinition(English, Colleagues, "Mr. %ContactFirstName%, I wish a merry Christmas with your family and friends and thank you for showing us that hard work really pays off!"),
                                   new MessageDefinition(English, Colleagues, "Ms. %ContactFirstName%, I wish a merry Christmas with your family and friends and thank you for showing us that hard work really pays off!"),
                                   new MessageDefinition(English, Colleagues, "Mr. %ContactFirstName%, thank you for your guidance in achieving goals and overcoming obstacles this past year. Merry Christmas, sir!"),
                                   new MessageDefinition(English, Colleagues, "Ms. %ContactFirstName%, thank you for your guidance in achieving goals and overcoming obstacles this past year. Merry Christmas, sir!"),
                                   new MessageDefinition(English, Colleagues, "Merry Christmas, %ContactFirstName%! Thank you for your work ethic and determination to achieve goals. I hope we continue to do great work together."),
                                   new MessageDefinition(English, Colleagues, "%ContactFirstName%, thank you for helping the team have great results this year. I wish you a merry Christmas with your family and friends!"),
                                   new MessageDefinition(English, Colleagues, "%ContactFirstName%, thank you for doing your bit and helping us have great results this year. I wish you a merry Christmas with your family and friends!"),
                                   new MessageDefinition(English, Colleagues, "Thank you for all your hard work and determination. May our collaboration be even more fruitful from now on. Merry Christmas!"),
                                   new MessageDefinition(English, Colleagues, "Merry Christmas, Mr. %ContactFirstName%. Thank you for all your hard work and determination. May our collaboration be even more fruitful from now on."),
                                   new MessageDefinition(English, Colleagues, "Merry Christmas, Ms. %ContactFirstName%. Thank you for all your hard work and determination. May our collaboration be even more fruitful from now on."),

                                   new MessageDefinition(English, Family, "Merry Christmas, %ContactFirstName%! I wish with all my heart for you to be happy and healthy and for me to have more time to tell you how much I love you!"),
                                   new MessageDefinition(English, Family, "Merry Christmas, %ContactFirstName%! I wish with all my heart for you to be happy and healthy and for me to have more time, so I can spend it with you!"),
                                   new MessageDefinition(English, Family, "My dear, this Christmas I have no special wishes for you, other than what I've been wanting all my life: for you to be happy. Love, dad."),
                                   new MessageDefinition(English, Family, "My son, all I want this Christmas is to see you filled with peace, sated with love and overwhelmed by happiness. Love, dad."),
                                   new MessageDefinition(English, Family, "My dear, this Christmas I have no special wishes for you, other than what I've been wanting all my life: for you to be happy. Love, mom."),
                                   new MessageDefinition(English, Family, "Dear son, this Christmas I have no special wishes for you, other than what I've been wanting all my life: for you to be happy. Love, mom."),
                                   new MessageDefinition(English, Family, "As I am counting my blessings this Christmas, I want you to know you're the top of my list. Thank you for being an amazing brother!"),
                                   new MessageDefinition(English, Family, "As I am counting my blessings this Christmas, I must admit my family is one of the best things in my life. Thank you for being an amazing sister!"),
                                   new MessageDefinition(English, Family, "I wish you a Christmas filled with the warmth of family joy of friends and peace of mind. I'm proud to be part of this family! Merry Christmas!"),
                                   new MessageDefinition(English, Family, "Merry Christmas, %ContactFirstName%! May your world be filled with warmth, peace and good cheer this Holidays season and throughout the year!"),
                                   
                                   new MessageDefinition(English, Friends, "Merry Christmas, %ContactFirstName%! I’m thankful to have people like you in my life to send all my good thoughts to on special occasions. Happy holidays!"),
                                   new MessageDefinition(English, Friends, "How to spread holiday cheer: be thankful for the good things in your life, like having a friend as awesome as I am! Merry Christmas, %ContactFirstName%!"),
                                   new MessageDefinition(English, Friends, "%ContactFirstName%, it seems like it's been an entire year since I've wished you a Merry Christmas. Maybe we should have Christmas more often! :)"),
                                   new MessageDefinition(English, Friends, "Merry Christmas to you and all your loved ones, %ContactFirstName%! May your holidays be filled with the true blessings of Christmas: peace, joy and hope!"),

                                   #endregion

                                   #region Romanian

                                   new MessageDefinition(Romanian, School, "Crăciun fericit, %ContactFirstName%! Fără tine, nu m-aș distra pe cinste la școală, așa că trebuie să-ți mulțumesc pentru asta! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, %ContactFirstName%! Bucură-te de vacanță cât mai mult și încearcă să ignori faptul că școala reîncepe în ianuarie! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, %ContactFirstName%! Fără tine, nu m-aș distra pe cinste la școală, așa că trebuie să-ți mulțumesc pentru asta! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, domnule %ContactFirstName%! Vă mulțumesc pentru răbdare și determinarea de a ne învăța lecții... de viață! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, doamna %ContactFirstName%! Vă mulțumesc pentru răbdare și determinarea de a ne învăța lecții... de viață! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, doamnă %ContactFirstName%! Vă doresc multe alte generații de elevi pe care să le ajutați. Vă mulțumesc că mi-ați fost profesoară!"),
                                   new MessageDefinition(Romanian, School, "Crăciun fericit, domnule %ContactFirstName%! Vă doresc multe alte generații de elevi pe care să le ajutați. Vă mulțumesc că mi-ați fost profesor!"),

                                   new MessageDefinition(Romanian, Friends, "Sărbători fericite, %ContactFirstName%! Mă bucur mult să am oameni ca tine în viața mea cărora să le transmit gânduri bune de Sărbători. Crăciun fericit!"),
                                   new MessageDefinition(Romanian, Friends, "Cum să intri în spiritul Sărbătorilor: mulțumește pentru lucrurile bune pe care le ai: cineva așa de minunat ca mine! Crăciun fericit, %ContactFirstName%!"),
                                   new MessageDefinition(Romanian, Friends, "%ContactFirstName%!, mi se pare că a trecut un an întreg de când ți-am urat Crăciun fericit ultima oară. Poate ar trebui să sărbătorim Crăciunul mai des! :)"),
                                   new MessageDefinition(Romanian, Friends, "Crăciun fericit ție și celor dragi! Sper ca Sărbătorile tale să fie pline de adevăratele binecuvântări ale Crăciunului: pace, bucurii și speranță!"),

                                   new MessageDefinition(Romanian, Colleagues, "Domnule %ContactFirstName%, vă mulțumesc pentru că sunteți un șef extraordinar și că ne-ați ajutat de fiecare dată. Crăciun fericit dvs. și celor dragi!"),
                                   new MessageDefinition(Romanian, Colleagues, "Doamnă %ContactFirstName%, vă mulțumesc pentru că sunteți un șef extraordinar și că ne-ați ajutat de fiecare dată. Crăciun fericit dvs. și celor dragi!"),
                                   new MessageDefinition(Romanian, Colleagues, "Domnule %ContactFirstName%, vă doresc un Crăciun fericit alături de familie și prieteni. Vă mulțumesc - mi-ați arătat că munca serioasă e răsplătită."),
                                   new MessageDefinition(Romanian, Colleagues, "Domnule %ContactFirstName%, vă mulțumesc pentru sprijinul acordat anul acesta în atingerea obiectivelor și depășirea obstacolelor. Crăciun fericit!"),
                                   new MessageDefinition(Romanian, Colleagues, "Doamnă %ContactFirstName%, vă mulțumesc pentru sprijinul acordat anul acesta în atingerea obiectivelor și depășirea obstacolelor. Crăciun fericit!"),
                                   new MessageDefinition(Romanian, Colleagues, "Crăciun fericit, %ContactFirstName%! Mulțumesc pentru seriozitate și toată munca depusă anul acesta. Să lucrăm cel puțin la fel de bine și anul următor!"),
                                   new MessageDefinition(Romanian, Colleagues, "%ContactFirstName%, mulțumesc că ai contribuit anul acesta la rezultatele grozave ale echipei! Crăciun fericit alături de familie și prieteni!"),
                                   new MessageDefinition(Romanian, Colleagues, "%ContactFirstName%!, mulțumesc pentru felul în care ți-ai făcut treaba și ai ajutat echipa anul acesta. Crăciun fericit alături de familie și cei dragi!"),
                                   new MessageDefinition(Romanian, Colleagues, "Mulțumesc pentru seriozitate și toată munca depusă anul acesta. Sper să colaborăm la fel de eficient și de acum înainte! Crăciun fericit!"),
                                   new MessageDefinition(Romanian, Colleagues, "Crăciun fericit, domnule %ContactFirstName%! Mulțumesc pentru seriozitate și munca depusă. Sper să colaborăm la fel de eficient și de acum înainte!"),
                                   new MessageDefinition(Romanian, Colleagues, "Crăciun fericit, doamnă %ContactFirstName%! Mulțumesc pentru seriozitate și munca depusă. Sper să colaborăm la fel de eficient și de acum înainte!"),

                                   new MessageDefinition(Romanian, Family, "Crăciun fericit, %ContactFirstName%! Îmi doresc din inimă multă fericire și sănătate pentru tine și mai mult timp pentru mine, ca să-ți spun cât te iubesc!"),
                                   new MessageDefinition(Romanian, Family, "Crăciun fericit, %ContactFirstName%! Îmi doresc din inimă multă fericire și sănătate pentru tine și mai mult timp pentru mine, ca să-l petrec cu tine!"),
                                   new MessageDefinition(Romanian, Family, "Draga mea, Crăciunul ăsta nu am nicio dorință specială, în afară de ceea ce am vrut toată viața pentru tine: să fii fericită. Te iubesc."),
                                   new MessageDefinition(Romanian, Family, "Fiule, Crăciunul acesta îmi doresc să te văd um om împăcat, cu iubire în suflet și copleșit de bucurii! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, Family, "Draga mea, Crăciunul ăsta nu am nicio dorință specială, în afară de ceea ce am vrut toată viața pentru tine: să fii fericită. Mama te iubește."),
                                   new MessageDefinition(Romanian, Family, "Dragul meu, Crăciunul ăsta nu am nicio dorință specială, în afară de ceea ce vrut toată viața pentru tine: să fii fericit. Mama te iubește."),
                                   new MessageDefinition(Romanian, Family, "Crăciunul ăsta mă gândesc la lucrurile pentru care sunt recunoscătoare, iar tu ești în fruntea listei! Mulțumesc pentru că ești un frate minunat!"),
                                   new MessageDefinition(Romanian, Family, "Crăciunul ăsta mă gândesc la lucrurile pentru care sunt recunoscător, iar tu ești în fruntea listei! Mulțumesc pentru că ești un frate minunat!"),
                                   new MessageDefinition(Romanian, Family, "Crăciunul ăsta mă gândesc la lucrurile pentru care sunt recunoscătoare, iar familia e pe primul loc! Mulțumesc pentru că ești o soră minunată!"),
                                   new MessageDefinition(Romanian, Family, "Crăciunul ăsta mă gândesc la lucrurile pentru care sunt recunoscător, iar familia e pe primul loc! Mulțumesc pentru că ești o soră minunată!"),
                                   new MessageDefinition(Romanian, Family, "Să aveți un Crăciun plin de căldura familiei, bucuria prietenilor și gânduri frumoase. Mă bucur să fac parte din familia asta! Sărbători fericite!"),
                                   new MessageDefinition(Romanian, Family, "Crăciun fericit, %ContactFirstName%!! Vă dorim liniște și pace sufletească, multe bucurii și inimi pline de speranță - de Sărbători și în anul ce urmează!")

                                   #endregion
                               };

            if (_configurationService.IsSMSSuffixEnabled)
            {                
                _messageCache.ForEach(
                    definition =>
                        {
                            definition.MessageTemplate = string.Join(
                                Environment.NewLine,
                                definition.MessageTemplate,
                                Properties.Resources.SentFromOmnipaste);
                        });
            }

            return Observable.Return(_messageCache);
        }
    }
}
