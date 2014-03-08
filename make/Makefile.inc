############ VARIABLES ############
REPO_ROOT=/Users/apoorvkulshrestha/Projects/SavariWala
PKG_ROOT=$(REPO_ROOT)/Server/ext/pkgs
CXX=g++
RM=rm -f

OBJS=$(subst .cpp,.o,$(SRCS))
CPPFLAGS += -ggdb3

all: $(BINARY) $(LIB)

############ MODULES ############

# Few modules are assumed installed for now instead of PKG_ROOT, namely: boost, thrift
#port install boost
ifeq ($(BOOST), yes)
    BOOST_DIR = /opt/local
    CPPFLAGS += -I$(BOOST_DIR)/include
    LDFLAGS += -L$(BOOST_DIR)/lib
    #LDLIBS += -l
endif

# installed from sources
ifeq ($(THRIFT), yes)
    THRIFT_DIR = /usr/local
    CPPFLAGS += -I$(THRIFT_DIR)/include
    LDFLAGS += -L$(THRIFT_DIR)/lib
    LDLIBS += -lthrift
endif

ifeq ($(GFLAGS), yes)
    GFLAGS_DIR = $(PKG_ROOT)/gflags/latest
    CPPFLAGS += -I$(GFLAGS_DIR)/include
    LDFLAGS += -L$(GFLAGS_DIR)/lib
    LDLIBS += -lgflags
endif

ifeq ($(GLOG), yes)
    GLOG_DIR = $(PKG_ROOT)/glog/latest
    CPPFLAGS += -I$(GLOG_DIR)/include
    LDFLAGS += -L$(GLOG_DIR)/lib
    LDLIBS += -lglog
endif

ifeq ($(CPPDB), yes)
    CPPDB_DIR = $(PKG_ROOT)/cppdb/latest
    CPPFLAGS += -I$(CPPDB_DIR)/include
    LDFLAGS += -L$(CPPDB_DIR)/lib
    LDLIBS += -lcppdb -lcppdb_postgresql
endif

ifeq ($(GENSW), yes)
    GENSW_DIR = $(REPO_ROOT)/idl
    CPPFLAGS += -I$(GENSW_DIR)
    LDFLAGS += -L$(GENSW_DIR)/gen-cpp
    LDLIBS += -lgensw
endif



############ VARIABLES ############

$(BINARY): $(OBJS) $(DEPLIBS)
	$(CXX) $(LDFLAGS) -o $(BINARY) $(OBJS) $(LDLIBS)

$(LIB) : $(OBJS)
	ar rcs $(LIB) $(OBJS)
	ranlib $(LIB)
 
%.o: %.cpp
	$(CXX) $(CPPFLAGS) -c $< -o $@

clean:
	$(RM) $(OBJS)
	$(RM) $(BINARY) $(LIB)

depend: .depend

.depend: $(SRCS)
	$(RM) .depend
	$(CXX) $(CPPFLAGS) -MM $^>>./.depend

-include .depend
