from bardapi import ChatBard
import os
os.environ["_BARD_API_KEY"] = 'XAj-rCuWmoytxF7NJG9UzgUmdplpgaWJ7hsDsTQgFsIdtjck5kc3RDFVSXx-3nDol6PPtA.'   # Requird
os.environ["_BARD_API_TIMEOUT"] = 30     # Optional, Session Timeout
 
chat = ChatBard()
chat.start()