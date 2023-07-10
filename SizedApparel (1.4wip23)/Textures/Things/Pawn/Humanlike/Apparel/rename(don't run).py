import os
  
files = os.listdir('./')
  
for file in files:
    if file[-2:] != 'py':
        name = file
        
        new_name = file.replace('_6', '_7')
        #new_name = file.replace('_5', '_6')
        #new_name = file.replace('_4', '_5')
        #new_name = file.replace('_3', '_4')
        #new_name = file.replace('_2', '_3')
        #new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)

for file in files:
    if file[-2:] != 'py':
        name = file
        
        #new_name = file.replace('_6', '_7')
        new_name = file.replace('_5', '_6')
        #new_name = file.replace('_4', '_5')
        #new_name = file.replace('_3', '_4')
        #new_name = file.replace('_2', '_3')
        #new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)
        
for file in files:
    if file[-2:] != 'py':
        name = file
        
        #new_name = file.replace('_6', '_7')
        #new_name = file.replace('_5', '_6')
        new_name = file.replace('_4', '_5')
        #new_name = file.replace('_3', '_4')
        #new_name = file.replace('_2', '_3')
        #new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)

for file in files:
    if file[-2:] != 'py':
        name = file
        
        #new_name = file.replace('_6', '_7')
        #new_name = file.replace('_5', '_6')
        #new_name = file.replace('_4', '_5')
        new_name = file.replace('_3', '_4')
        #new_name = file.replace('_2', '_3')
        #new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)
        
for file in files:
    if file[-2:] != 'py':
        name = file
        
        #new_name = file.replace('_6', '_7')
        #new_name = file.replace('_5', '_6')
        #new_name = file.replace('_4', '_5')
        #new_name = file.replace('_3', '_4')
        new_name = file.replace('_2', '_3')
        #new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)

for file in files:
    if file[-2:] != 'py':
        name = file
        
        #new_name = file.replace('_6', '_7')
        #new_name = file.replace('_5', '_6')
        #new_name = file.replace('_4', '_5')
        #new_name = file.replace('_3', '_4')
        #new_name = file.replace('_2', '_3')
        new_name = file.replace('_1', '_2')
        
        name = os.path.join('./', name)
        new_name = os.path.join('./', new_name)
        os.rename(name, new_name)
        
