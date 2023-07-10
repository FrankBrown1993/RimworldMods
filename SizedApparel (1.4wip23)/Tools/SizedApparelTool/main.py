import sys
import math
from PyQt5.QtCore import QPoint
from PyQt5.QtWidgets import QApplication, QWidget, QPushButton, QToolTip , QMainWindow, QAction, qApp , QHBoxLayout, QVBoxLayout, QFrame
from PyQt5.QtWidgets import QRadioButton, QCheckBox, QGroupBox, QGridLayout, QMenu, QTabWidget, QDesktopWidget, QLabel, QStatusBar
from PyQt5.QtGui import QPixmap
from PyQt5.QtGui import QFont
from PyQt5.QtGui import QIcon
from PyQt5.QtCore import QCoreApplication



class MyApp(QWidget):
    graphic_image = QPixmap
    lbl_img = QLabel

    def __init__(self):
        super().__init__()
        self.setWindowTitle('ImagePointFinder')
        self.statusbar = QStatusBar()
        self.statusbar.showMessage("Click Image to Get Position")
        self.initUI()
        self.setAcceptDrops(True)
        self.posX = 0.0
        self.posY = 0.0
        self.Rot = 0.0
        self.Len = 0

        #self.setStatusBar(self.statusbar)

    def dragEnterEvent(self, event):
        if event.mimeData().hasUrls():
            event.accept()
        else:
            event.ignore()

    def dropEvent(self, event):
        files = [u.toLocalFile() for u in event.mimeData().urls()]
        for f in files:
            print(f)
        self.graphic_image = QPixmap(files[0])

        self.lbl_img.setPixmap(self.graphic_image.scaled(512,512))
        #self.lbl_img.frame = QFrame(self.lbl_img.frame)
        self.lbl_img.setFrameShape(QFrame.Box)

    def mousePressEvent(self, event):
        #event.x()

        #print()
        self.posX = event.globalX() - self.lbl_img.mapToGlobal(QPoint(0,0)).x()
        self.posY = event.globalY() - self.lbl_img.mapToGlobal(QPoint(0,0)).y()
        self.Rot = 0.0
        self.Len = 0.0
        U = 0
        V = 0
        if(self.lbl_img.pixmap()):
            U = (self.posX / self.lbl_img.pixmap().height())-0.5
            V = (self.posY / self.lbl_img.pixmap().width())*-1 + 0.5
            txt = "Clicked Position ; Position(x,y) =({0:.3f}, {1:.3f}) , Rotation = {2:.1f}, Length = {3:.3f} ".format(U, V, self.Rot, self.Len)
            self.statusbar.showMessage(txt)
        #print(event.globalX())

    def mouseMoveEvent(self, event):
        target_posX = event.globalX() - self.lbl_img.mapToGlobal(QPoint(0,0)).x()
        target_posY = event.globalY() - self.lbl_img.mapToGlobal(QPoint(0,0)).y()

        self.Rot = math.degrees( math.atan2(target_posY - self.posY, target_posX - self.posX)) + 90
        if(self.Rot < 0):
            self.Rot = self.Rot + 360


        if(self.lbl_img.pixmap()):
            U = (self.posX / self.lbl_img.pixmap().height())-0.5
            V = (self.posY / self.lbl_img.pixmap().width())*-1 + 0.5
            tU = target_posX / self.lbl_img.pixmap().height()
            tV = target_posY / self.lbl_img.pixmap().width()
            self.Len = math.fabs(math.dist((U,V),( tU, tV)))
            txt = "Clicked Position ; Position(x,y) =({0:.3f}, {1:.3f}) , Rotation = {2:.1f}, Length = {3:.3f} ".format(U, V, self.Rot, self.Len)
            self.statusbar.showMessage(txt)

    def initUI(self):
        tab1 = QWidget()
        tab2 = QWidget()

        tabs = QTabWidget()
        tabs.addTab(tab1, 'Texture Pivot and Point Editor')
        #tabs.addTab(tab2, 'Sized Texture List Editor')

        vbox = QVBoxLayout()
        vbox.addWidget(tabs)

        self.graphic_image = QPixmap()
        self.lbl_img = QLabel()
        #self.lbl_img.setPixmap(self.graphic_image)
        self.lbl_img.setText("Drag and Drop Image Here")

        vbox2 = QVBoxLayout()
        vbox2.addWidget(self.lbl_img)
        vbox2.addWidget(self.statusbar)
        tab1.setLayout(vbox2)


        self.setLayout(vbox)

        #print(self.hasMouseTracking())
        self.setMouseTracking(True)

        #print(self.hasMouseTracking())


        self.setGeometry(500, 500, 500, 400)
        self.center()

        self.show()
        #self.statusbar.show()

    def center(self):
        qr = self.frameGeometry()
        cp = QDesktopWidget().availableGeometry().center()
        qr.moveCenter(cp)
        self.move(qr.topLeft())


if __name__ == '__main__':
    app = QApplication(sys.argv)
    ex = MyApp()
    sys.exit(app.exec_())