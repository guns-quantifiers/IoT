import PySimpleGUI as sg

sg.change_look_and_feel('DarkBlue1')

class ConfigWindow:

    def __init__(self, params):
        self.params = params

    def run(self):
        layout = [
            [sg.Text('System parameters:')],
            [sg.Text('Card min size:', size=(15, 1)), sg.InputText(1000, key="minSize")],
            [sg.Text('Card max size', size=(15, 1)), sg.InputText(25000, key='maxSize')],
            [sg.Text('X deviation', size=(15, 1)), sg.InputText(5, key='xDeviation')],
            [sg.Text('Y deviation', size=(15, 1)), sg.InputText(5, key='yDeviation')],
            [sg.Text('Threshold base', size=(15, 1)), sg.InputText(5, key='threshAdder')],
            [sg.Button('Update')]
        ]
        self.window = sg.Window('Config window', layout)

        while True:
            event, values = self.window.read() 
            if event == "Update":
                print(values)
                self.update(values)
            if event in (None, 'Exit'):      
                break      
 
    
    def update(self, values):
        print(values)
        self.params.minSize = float(values['minSize'])
        self.params.maxSize = float(values['maxSize'])
        self.params.xDeviation = int(values['xDeviation'])
        self.params.yDeviation = int(values['yDeviation'])
        self.params.threshAdder = int(values['threshAdder'])
