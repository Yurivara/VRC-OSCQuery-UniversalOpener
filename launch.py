#Opens and states the usable port then runs the OSC Program.
import subprocess
import json

with open('VRC-OSCQuery-UniversalOpener\\config.json') as file:
    config = json.load(file)


process = subprocess.Popen(
	["VRC-OSCQuery-UniversalOpener\\Program.cs", shell = True],
	stdout=subprocess.PIPE,
    stderr=subprocess.STDOUT,
    text=True,
    bufsize=1
)

for line in process.stdout:
    if line.strip() == "OSCQuery Service Opened":
        print("OSCQuery service opened.\nRunning Main OSC Application.")
        subprocess.run(config["name"], shell = True)