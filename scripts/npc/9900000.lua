addText('Do you like Destiny scripting interface?')
answer = askYesNo()

if answer == answer_yes then
    addText('Thanks!')
else
    addText('Boo!')
end

sendOk()