import matplotlib.pyplot as plt
import os
figdest = "C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Analysis/plot.png"
file = open("C:/Users/jskrz/Desktop/ASK/ASK PROJEKT 5/Psychomotor Test/Psychomotor Test/Analysis/Anal.txt", "r+")

time = []
probes = []

for line in file:
    time.append(float(line))
    probes.append(len(time))

file.close()

plt.plot(probes,time)
if os.path.isfile(figdest):
   os.remove(figdest)

plt.savefig(figdest)
