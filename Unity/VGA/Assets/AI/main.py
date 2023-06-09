from flask import Flask
from bardapi import Bard
import random

def AI(mode,input):
    tokens = ['Wgjd4MqyxYSNu5qTXY9WL6zxH8H26G6thLxhjw3_t89Oi_WU-sxu9itNmIz4gOQSD9alzw.','XAj-rCuWmoytxF7NJG9UzgUmdplpgaWJ7hsDsTQgFsIdtjck5kc3RDFVSXx-3nDol6PPtA.']
    bard = Bard(token=tokens[random.randint(0, len(tokens)-1)])
    text = "prompt"
    if (mode != "draw"):
        text+= "_exp.txt"
    else:
        text+= "_draw.txt"
    prompt= open(text,"r").read()
    output= bard.get_answer("**Answer the following question in short form(do not say sure and this is the shorted form, send dircitly your answer):" +input)['content']
    if ("Erorr" or "erorr" in output):
        output = "Erorr, try after a minute"
    return output

app = Flask(__name__)

@app.route("/")
def home():
    return 'this is an A for VGA project!<br><br>Usage:<br>\thttp://ourdomain.tld/[mode],text=[text]<br><br> Modes:"exp","draw"'

@app.route("/<input>")
def api(input):
    arr = str(input).split(',text=')
    if (len(arr) is 2):
        return AI(str(arr[0]), str(arr[1]))
    else:
        print("!!!!!!!!!!!!!!!OUTPUT:"+input)
        return "Erorr!"
        

if __name__ == "__main__":
    app.run()