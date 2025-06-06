#include <conio.h>
#include <stdio.h>
#include <cstdlib>
#include <iostream>
using namespace std;

/*struct str_choices
{
    string user;
    string computer;
    string winner;
};*/
enum enchoices{paper=1,sciscor,stone};
enum enwinner{win=1,lose=2,draw=3};
struct Stgame
{
    int round=0;
    enchoices user;
    enchoices computer;
    enwinner winner;
    string str_winner;

};
struct stresult
{
    int round=0;
    enwinner winner;int count_user=0;int count_computer=0;int count_draw=0;
    //string finalwinner;

};
int randomnumber(int from,int to)
{
    return rand()%(to-from+1)+from;
}
int readpositvenumber(string message)
{
    int n;
    do
    {
        cout<<message;
        cin>>n;
    } while (n<=0);
   return n; 
}
int howmanyround(int num)
{
    return num;
}
enchoices user_choice(string message)
{
 int choice;
 do
 {
    cout<<message;
    cin>>choice;
    
 } while (choice<1||choice>3);
    
 
 return enchoices(choice);

}
enchoices computer_choice()
{
 int choice=randomnumber(1,3);   
 

 return enchoices(choice);

}
enwinner discusschoices(Stgame &game)
{
  
 if(game.user==game.computer)
 return enwinner::draw; 
 if(game.user==enchoices::paper)
 {
   if(game.computer==enchoices::sciscor)
   game.winner=enwinner::lose;
   else
   game.winner=enwinner::win;
 }
 else if(game.user==enchoices::sciscor)
 {
    if(game.computer==enchoices::paper)
    game.winner=enwinner::win;
    else
    game.winner=enwinner::lose;
 }
 else 
 {
    if(game.computer==enchoices::sciscor)
    game.winner=enwinner::win;
    else
    game.winner=enwinner::lose;
    
    
 }
 return game.winner; 
}
string str_choice(enchoices choice)
{
  string arr[3]{"Paper","Sciscors","Stone"};
  return arr[choice-1];
}
string str_winner(enwinner winner)
{
    string arr[3]{"User","computer","No winner"};
    return arr[winner-1];
}
void setscreencolor(enwinner winner)
{
        if(winner==enwinner::win)
        {
            system("color 2F");
            //game.str_winner="Round winner :"+str_winner(game.winner)+"\n";
        }
        else if(winner==enwinner::lose)
        {
            
            system("color 4F");
            cout<<"\a";

            //game.str_winner="Round winner :"+str_winner(game.winner)+"\n";
        }
        else
        {
        system("color 5F");    
        //game.str_winner="Round winner :"+str_winner(game.winner)+"\n";
        //return ch;
        }

}


void count(enwinner winner,stresult &result)
{

    if(winner==enwinner::win)
    result.count_user++;
    else if(winner==enwinner::lose)
    result.count_computer++;
    else
    result.count_draw++;
}
void gameover()
{
  cout<<"\t------------------------------------------\n";
    cout<<"\t\t+++Game over+++\n";
    cout<<"\t------------------------------------------\n";
}
enwinner whowonthefinalgame(stresult &result)
{
    if(result.count_user==result.count_computer)
    result.winner=enwinner::draw;
    else if(result.count_user>result.count_computer)
    result.winner=enwinner::win;
    else
    result.winner=enwinner::lose;
    return result.winner; 
}
void gameresult(stresult result)
{
    result.winner=whowonthefinalgame(result);
    
    //string final_result;
    gameover();
    cout<<"\t-----------------Game result-------------------\n";
    cout<<"\tGame round :"<<result.round<<endl;
    cout<<"\tuser won times :"<<result.count_user<<endl;
    cout<<"\tcomputer won time :"<<result.count_computer<<endl;
    cout<<"\tdraw times :"<<result.count_draw<<endl;
    cout<<"\tfinal winner :"<<str_winner(result.winner); 
    cout<<"\n\t------------------------------------------\n";
    //resetscreen(result.winner);
    enwinner win=result.winner;
    setscreencolor(win);

}
void roundsprint(Stgame &game)
{
    
    stresult result;
    //int count_user=0,count_computer=0,count_draw=0;
    game.round=howmanyround(readpositvenumber("enter how many round "));
    for(int i=1;i<=game.round;i++)
    {
        cout<<"Round ["<<i<<"] begin :\n";
        game.user=user_choice("Your choice: [1]paper and [2]sciscor and [3]stone ");
        game.computer=computer_choice();
        game.winner=discusschoices(game);
        game.str_winner=str_winner(game.winner);
        cout<<"---------Round ["<<i<<"]-----------\n";
        setscreencolor(game.winner);
        cout<<"user choice :"<<str_choice(game.user)<<endl<<"computer choice :"<<str_choice(game.computer)<<endl<<"Round winner :"<<str_winner(game.winner)<<endl;
        cout<<"-----------------------------\n";
        count(game.winner,result);      
         

    }
       
    result.winner=whowonthefinalgame(result);
    //count(result);
    
    result.round=game.round;

    gameresult(result);
}
void resetscreen()
{
     system("cls");
     system("color 0F");
}
void playagain(string message)
{
    char yesorno;
    do
    {
        
        
        Stgame game;
        roundsprint(game); 
        cout<<message; 
        cin>>yesorno;
        resetscreen();
        
        

        
        
    
    } while (yesorno=='Y'||yesorno=='y');
    
}