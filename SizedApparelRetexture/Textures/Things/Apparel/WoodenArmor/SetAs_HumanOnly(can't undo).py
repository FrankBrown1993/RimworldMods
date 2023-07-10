import os

for filename in os.listdir("."):
    new_filename = filename.replace("_Female", "_Female_Human")
    new_filename = new_filename.replace("_Thin", "_Thin_Human")
    new_filename = new_filename.replace("_Male", "_Male_Human")
    new_filename = new_filename.replace("_Fat", "_Fat_Human")
    new_filename = new_filename.replace("_Hulk", "_Hulk_Human")
    os.rename(filename, new_filename)
