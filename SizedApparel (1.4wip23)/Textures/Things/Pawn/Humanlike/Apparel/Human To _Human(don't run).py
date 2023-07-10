import os

for filename in os.listdir("."):
    new_filename = filename.replace("Human", "_Human")

    os.rename(filename, new_filename)
