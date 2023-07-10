import os

for filename in os.listdir("."):
    new_filename = filename.replace("_Female", "Human_Female")
    new_filename = new_filename.replace("_Thin", "Human_Thin")
    new_filename = new_filename.replace("_Male", "Human_Male")
    new_filename = new_filename.replace("_Fat", "Human_Fat")
    new_filename = new_filename.replace("_Hulk", "Human_Hulk")
    os.rename(filename, new_filename)
