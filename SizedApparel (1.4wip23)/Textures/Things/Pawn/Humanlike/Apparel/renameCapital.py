import os

for filename in os.listdir("."):
    new_filename = filename.replace("_female", "_Female")
    new_filename = new_filename.replace("_thin", "_Thin")
    new_filename = new_filename.replace("_male", "_Male")
    new_filename = new_filename.replace("_fat", "_Fat")
    new_filename = new_filename.replace("_hulk", "_Hulk")
    os.rename(filename, new_filename)
